import { zodResolver } from "@hookform/resolvers/zod";
import { ExternalLink, Pencil, Send, Trash2 } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useChannelConnection } from "@/domains/channel-connections/hooks";
import {
  useConversation,
  useConversationMessageList,
  useCreateConversationMessage,
  useDeleteConversation,
  useDirectTenantId,
} from "@/domains/direct/hooks";
import {
  conversationMessageFormSchema,
  type ConversationMessageFormValues,
} from "@/domains/direct/schemas";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import {
  DetailLayout,
  KeyValueList,
  PageHeader,
} from "@/shared/layout/page-layouts";
import {
  ConversationStatus,
  MessageActorKind,
  backendEnumSelectOptions,
  messageActorKindLabels,
} from "@/shared/constants/backend-enums";
import {
  dateTimeLocalInputToUtc,
  toDateTimeLocalInputValue,
} from "@/shared/lib/date-time-input";
import { translateText } from "@/shared/lib/i18n-core";
import { formatOptionalDateTimeInTimeZone } from "@/shared/lib/time-zone";
import {
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardHeading,
  CardTitle,
  ConfirmAction,
  Form,
  FormSectionHeading,
} from "@/shared/ui";
import { SelectField, TextareaField, TextField } from "@/shared/ui/form-fields";
import { PaginationControls } from "@/shared/ui/pagination-controls";
import { ErrorState } from "@/shared/ui/placeholder-state";
import {
  ChannelConnectionKindBadge,
  ChannelConnectionStatusBadge,
  ConversationStatusBadge,
  MessageActorKindBadge,
} from "@/shared/ui/status-badges";

const actorOptions = backendEnumSelectOptions(messageActorKindLabels);

export function ConversationDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const tenantId = useDirectTenantId();
  const timeZone = usePortalTimeZone();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const conversationQuery = useConversation(id);
  const messagesQuery = useConversationMessageList({
    conversationId: id ?? "",
    page,
    pageSize,
    sorting: "SentAtUtc DESC",
  });
  const connectionQuery = useChannelConnection(
    conversationQuery.data?.channelConnectionId,
  );
  const createMessage = useCreateConversationMessage();
  const deleteConversation = useDeleteConversation();
  const form = useForm<ConversationMessageFormValues>({
    resolver: zodResolver(conversationMessageFormSchema),
    defaultValues: {
      actorKind: MessageActorKind.Agent,
      body: "",
      sentAtLocal: toDateTimeLocalInputValue(),
    },
  });
  const data = conversationQuery.data;
  const contact = data?.contact;
  const contactName = contact
    ? [contact.givenName, contact.surname].filter(Boolean).join(" ")
    : translateText("Unknown contact");
  const isOpen = data?.status === ConversationStatus.Open;

  return (
    <DetailLayout
      header={
        <PageHeader
          title={data?.subject || "Conversation"}
          description="Review customer context and the complete Direct message timeline."
          descriptionMode="hint"
          backTo="/app/direct/conversations"
          actions={
            data ? (
              <>
                <Button asChild variant="outline">
                  <Link to={`/app/direct/conversations/${data.id}/edit`}>
                    <Pencil className="size-4" />
                    {translateText("Edit")}
                  </Link>
                </Button>
                <ConfirmAction
                  title={translateText("Delete this conversation?")}
                  description={translateText(
                    "Only conversations without messages can be deleted. Close it instead when history must be retained.",
                  )}
                  confirmLabel={translateText("Delete conversation")}
                  isPending={deleteConversation.isPending}
                  onConfirm={async () => {
                    await deleteConversation.mutateAsync(data.id);
                    navigate("/app/direct/conversations");
                  }}
                  trigger={
                    <Button variant="outline">
                      <Trash2 className="size-4 text-destructive" />
                      {translateText("Delete")}
                    </Button>
                  }
                />
              </>
            ) : undefined
          }
        />
      }
      sidebar={
        data ? (
          <>
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Conversation summary</CardTitle>
                </CardHeading>
              </CardHeader>
              <CardContent>
                <KeyValueList
                  items={[
                    {
                      label: "Status",
                      value: <ConversationStatusBadge status={data.status} />,
                    },
                    { label: "Contact", value: contactName },
                    { label: "Messages", value: String(data.messageCount) },
                    {
                      label: "Last message",
                      value: formatOptionalDateTimeInTimeZone(
                        data.lastMessageAtUtc,
                        timeZone,
                        translateText("No messages"),
                      ),
                    },
                    {
                      label: "Created",
                      value: formatOptionalDateTimeInTimeZone(
                        data.createdAtUtc,
                        timeZone,
                        translateText("Unknown"),
                      ),
                    },
                  ]}
                />
              </CardContent>
            </Card>
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Channel connection</CardTitle>
                  <CardDescription>
                    Provider account used for this exchange.
                  </CardDescription>
                </CardHeading>
              </CardHeader>
              <CardContent>
                {connectionQuery.data ? (
                  <KeyValueList
                    items={[
                      { label: "Name", value: connectionQuery.data.name },
                      {
                        label: "Kind",
                        value: (
                          <ChannelConnectionKindBadge
                            kind={connectionQuery.data.kind}
                          />
                        ),
                      },
                      {
                        label: "Status",
                        value: (
                          <ChannelConnectionStatusBadge
                            status={connectionQuery.data.status}
                          />
                        ),
                      },
                    ]}
                  />
                ) : (
                  <p className="text-sm text-muted-foreground">
                    {translateText("Connection details are unavailable.")}
                  </p>
                )}
              </CardContent>
            </Card>
            {contact ? (
              <Card>
                <CardHeader>
                  <CardHeading>
                    <CardTitle>Contact context</CardTitle>
                  </CardHeading>
                </CardHeader>
                <CardContent className="space-y-4">
                  <KeyValueList
                    items={[
                      {
                        label: "Email",
                        value: contact.email ?? "Not provided",
                      },
                      {
                        label: "Phone number",
                        value: contact.phoneNumber ?? "Not provided",
                      },
                      {
                        label: "Time zone",
                        value: contact.timeZone ?? "Not provided",
                      },
                    ]}
                  />
                  {contact.instagramProfileUrl ? (
                    <Button asChild variant="outline" className="w-full">
                      <a
                        href={contact.instagramProfileUrl}
                        target="_blank"
                        rel="noreferrer"
                      >
                        <ExternalLink className="size-4" />
                        {translateText("Open Instagram profile")}
                      </a>
                    </Button>
                  ) : null}
                </CardContent>
              </Card>
            ) : null}
          </>
        ) : undefined
      }
    >
      {!tenantId ? (
        <TenantRequiredState />
      ) : conversationQuery.isError ? (
        <ErrorState
          title="Unable to load conversation"
          error={conversationQuery.error}
          retry={() => void conversationQuery.refetch()}
        />
      ) : (
        <>
          {isOpen ? (
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Add message</CardTitle>
                  <CardDescription>
                    Append a sourced message to the immutable conversation
                    timeline.
                  </CardDescription>
                </CardHeading>
              </CardHeader>
              <CardContent>
                <Form {...form}>
                  <form
                    className="space-y-5"
                    onSubmit={form.handleSubmit(async (values) => {
                      await createMessage.mutateAsync({
                        conversationId: id ?? "",
                        actorKind: values.actorKind,
                        body: values.body.trim(),
                        sentAtUtc: dateTimeLocalInputToUtc(values.sentAtLocal),
                      });
                      form.reset({
                        actorKind: values.actorKind,
                        body: "",
                        sentAtLocal: toDateTimeLocalInputValue(),
                      });
                      setPage(1);
                    })}
                  >
                    <FormSectionHeading
                      title="Message evidence"
                      description="Record who authored the message, its exact body, and the provider timestamp."
                    />
                    <div className="grid gap-5 md:grid-cols-2">
                      <SelectField
                        control={form.control}
                        name="actorKind"
                        label="Actor kind"
                        description="Role that authored this message: contact, user, automation, agent, or system."
                        options={actorOptions}
                      />
                      <TextField
                        control={form.control}
                        name="sentAtLocal"
                        type="datetime-local"
                        label="Sent time"
                        description="Local representation of the provider send time; stored as UTC."
                      />
                    </div>
                    <TextareaField
                      control={form.control}
                      name="body"
                      label="Message body"
                      description="Exact message content captured from or sent through the selected channel."
                      rows={5}
                    />
                    <Button type="submit" disabled={createMessage.isPending}>
                      <Send className="size-4" />
                      {translateText("Add message")}
                    </Button>
                  </form>
                </Form>
              </CardContent>
            </Card>
          ) : data ? (
            <div className="rounded-lg border border-border bg-muted/25 p-4 text-sm text-muted-foreground">
              {translateText(
                "This conversation is closed. Reopen it before adding another message.",
              )}
            </div>
          ) : null}

          {messagesQuery.isError ? (
            <ErrorState
              title="Unable to load messages"
              error={messagesQuery.error}
              retry={() => void messagesQuery.refetch()}
            />
          ) : (
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Message timeline</CardTitle>
                  <CardDescription>
                    Newest provider event first. Sent time is distinct from
                    persistence time.
                  </CardDescription>
                </CardHeading>
              </CardHeader>
              <CardContent className="space-y-5">
                {messagesQuery.isLoading ? (
                  <p className="py-8 text-center text-sm text-muted-foreground">
                    {translateText("Loading messages...")}
                  </p>
                ) : messagesQuery.data?.items.length ? (
                  <div className="divide-y divide-border rounded-lg border border-border">
                    {messagesQuery.data.items.map((message) => (
                      <article
                        key={message.id}
                        className="space-y-3 p-4 sm:p-5"
                      >
                        <div className="flex flex-wrap items-center justify-between gap-2">
                          <MessageActorKindBadge kind={message.actorKind} />
                          <time className="text-xs text-muted-foreground">
                            {formatOptionalDateTimeInTimeZone(
                              message.sentAtUtc,
                              timeZone,
                              translateText("Unknown"),
                            )}
                          </time>
                        </div>
                        <p className="whitespace-pre-wrap break-words text-sm leading-6 text-foreground">
                          {message.body}
                        </p>
                      </article>
                    ))}
                  </div>
                ) : (
                  <div className="py-10 text-center">
                    <p className="font-medium text-mono">
                      {translateText("No messages yet")}
                    </p>
                    <p className="mt-1 text-sm text-muted-foreground">
                      {translateText(
                        "The first captured message will appear here.",
                      )}
                    </p>
                  </div>
                )}
                {messagesQuery.data ? (
                  <PaginationControls
                    page={page}
                    pageSize={pageSize}
                    totalCount={messagesQuery.data.totalCount}
                    onPageChange={setPage}
                    onPageSizeChange={(nextPageSize) => {
                      setPageSize(nextPageSize);
                      setPage(1);
                    }}
                    isFetching={messagesQuery.isFetching}
                  />
                ) : null}
              </CardContent>
            </Card>
          )}
        </>
      )}
    </DetailLayout>
  );
}
