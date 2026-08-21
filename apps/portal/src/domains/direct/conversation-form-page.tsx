import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect, useMemo, useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Cable, Save, X } from "lucide-react";
import {
  useChannelConnection,
  useChannelConnectionList,
} from "@/domains/channel-connections/hooks";
import {
  useContactList,
  useConversation,
  useCreateConversation,
  useDirectTenantId,
  useUpdateConversation,
} from "@/domains/direct/hooks";
import {
  conversationFormSchema,
  type ConversationFormValues,
} from "@/domains/direct/schemas";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import {
  DetailLayout,
  KeyValueList,
  PageHeader,
} from "@/shared/layout/page-layouts";
import {
  ChannelConnectionStatus,
  ConversationStatus,
  backendEnumSelectOptions,
  conversationStatusLabels,
} from "@/shared/constants/backend-enums";
import { translateText } from "@/shared/lib/i18n-core";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardHeading,
  CardTitle,
  Form,
  FormCardSkeleton,
  FormSectionHeading,
} from "@/shared/ui";
import {
  SearchSelectField,
  SelectField,
  TextareaField,
} from "@/shared/ui/form-fields";
import { ErrorState } from "@/shared/ui/placeholder-state";
import { ConversationStatusBadge } from "@/shared/ui/status-badges";

const statusOptions = backendEnumSelectOptions(conversationStatusLabels);

export function ConversationFormPage({ mode }: { mode: "create" | "edit" }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const tenantId = useDirectTenantId();
  const [contactSearch, setContactSearch] = useState("");
  const conversationQuery = useConversation(mode === "edit" ? id : undefined);
  const contactsQuery = useContactList({
    page: 1,
    pageSize: 50,
    sorting: "Name ASC",
    searchText: contactSearch.trim() || undefined,
  });
  const connectionsQuery = useChannelConnectionList({
    page: 1,
    pageSize: 100,
    sorting: "Name ASC",
    status: ChannelConnectionStatus.Connected,
    isEnabled: true,
  });
  const currentConnectionQuery = useChannelConnection(
    conversationQuery.data?.channelConnectionId,
  );
  const createConversation = useCreateConversation();
  const updateConversation = useUpdateConversation(id ?? "");
  const form = useForm<ConversationFormValues>({
    resolver: zodResolver(conversationFormSchema),
    defaultValues: {
      contactId: "",
      channelConnectionId: "",
      subject: "",
      status: ConversationStatus.Open,
    },
  });

  useEffect(() => {
    if (!conversationQuery.data) {
      return;
    }
    form.reset({
      contactId: conversationQuery.data.contactId,
      channelConnectionId: conversationQuery.data.channelConnectionId,
      subject: conversationQuery.data.subject ?? "",
      status: conversationQuery.data.status,
    });
  }, [conversationQuery.data, form]);

  const selectedContactId = form.watch("contactId");
  const selectedConnectionId = form.watch("channelConnectionId");
  const contactOptions = useMemo(
    () =>
      (contactsQuery.data?.items ?? []).map((contact) => ({
        value: contact.id,
        label: [contact.givenName, contact.surname].filter(Boolean).join(" "),
        description:
          contact.email ?? contact.phoneNumber ?? "No contact method",
      })),
    [contactsQuery.data?.items],
  );
  const loadedContact = conversationQuery.data?.contact;
  const selectedContactOption =
    contactOptions.find((option) => option.value === selectedContactId) ??
    (loadedContact && loadedContact.id === selectedContactId
      ? {
          value: loadedContact.id,
          label: [loadedContact.givenName, loadedContact.surname]
            .filter(Boolean)
            .join(" "),
          description:
            loadedContact.email ??
            loadedContact.phoneNumber ??
            "No contact method",
        }
      : null);
  const connectionOptions = useMemo(
    () =>
      (connectionsQuery.data?.items ?? []).map((connection) => ({
        value: connection.id,
        label: connection.name,
        description: connection.providerKey,
      })),
    [connectionsQuery.data?.items],
  );
  const loadedConnection = currentConnectionQuery.data;
  const selectedConnectionOption =
    connectionOptions.find((option) => option.value === selectedConnectionId) ??
    (loadedConnection && loadedConnection.id === selectedConnectionId
      ? {
          value: loadedConnection.id,
          label: loadedConnection.name,
          description: loadedConnection.providerKey,
        }
      : null);
  const backTo = id
    ? `/app/direct/conversations/${id}`
    : "/app/direct/conversations";
  const data = conversationQuery.data;

  return (
    <DetailLayout
      header={
        <PageHeader
          title={
            mode === "create"
              ? "New conversation"
              : data?.subject
                ? `${translateText("Edit")} ${data.subject}`
                : "Edit conversation"
          }
          description="Connect one customer identity to one available provider channel."
          descriptionMode="hint"
          backTo={backTo}
          actions={
            <Button asChild variant="outline">
              <Link to="/app/settings/channel-connections">
                <Cable className="size-4" />
                {translateText("Channel connections")}
              </Link>
            </Button>
          }
        />
      }
      sidebar={
        data ? (
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
                  { label: "Messages", value: String(data.messageCount) },
                  {
                    label: "Contact",
                    value: [data.contact.givenName, data.contact.surname]
                      .filter(Boolean)
                      .join(" "),
                  },
                  {
                    label: "Channel",
                    value: currentConnectionQuery.data?.name ?? "Unavailable",
                  },
                ]}
              />
            </CardContent>
          </Card>
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
      ) : mode === "edit" && conversationQuery.isLoading ? (
        <FormCardSkeleton fields={4} />
      ) : (
        <Card>
          <CardHeader>
            <CardHeading>
              <CardTitle>Conversation details</CardTitle>
            </CardHeading>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                className="space-y-6"
                onSubmit={form.handleSubmit(async (values) => {
                  const body = {
                    ...values,
                    subject: values.subject?.trim() || undefined,
                  };
                  const conversationId =
                    mode === "create"
                      ? await createConversation.mutateAsync(body)
                      : await updateConversation.mutateAsync(body);
                  navigate(`/app/direct/conversations/${conversationId}`);
                })}
              >
                <FormSectionHeading
                  title="Routing"
                  description="Choose the customer and connected channel that own this exchange."
                />
                <div className="grid gap-5 lg:grid-cols-2">
                  <SearchSelectField
                    control={form.control}
                    name="contactId"
                    label="Contact"
                    description="Customer identity whose history and profile context belong to this conversation."
                    options={contactOptions}
                    selectedOption={selectedContactOption}
                    placeholder="Select a contact"
                    searchPlaceholder="Search contacts"
                    emptyMessage="No contacts found"
                    loading={contactsQuery.isFetching}
                    searchValue={contactSearch}
                    onSearchChange={setContactSearch}
                  />
                  <SearchSelectField
                    control={form.control}
                    name="channelConnectionId"
                    label="Channel connection"
                    description="Connected and enabled provider account used to exchange these messages."
                    options={connectionOptions}
                    selectedOption={selectedConnectionOption}
                    placeholder="Select a connected channel"
                    searchPlaceholder="Search channel connections"
                    emptyMessage="No connected channels found"
                    loading={connectionsQuery.isFetching}
                  />
                </div>

                <FormSectionHeading
                  title="Work state"
                  description="Give operators enough context to scan and complete the conversation."
                />
                <TextareaField
                  control={form.control}
                  name="subject"
                  label="Subject"
                  description="Optional concise summary of the customer's need; limited to 500 characters."
                  rows={3}
                />
                <div className="max-w-md">
                  <SelectField
                    control={form.control}
                    name="status"
                    label="Conversation status"
                    description="Open conversations accept messages; closed conversations preserve a read-only timeline."
                    options={statusOptions}
                    confirmation={{
                      title: "Change conversation status?",
                      description:
                        "Closing makes the message timeline read-only. Reopen only when active customer work resumes.",
                      confirmLabel: "Change status",
                    }}
                  />
                </div>

                <div className="flex flex-wrap gap-3">
                  <Button
                    type="submit"
                    disabled={
                      createConversation.isPending ||
                      updateConversation.isPending
                    }
                  >
                    <Save className="size-4" />
                    {translateText(
                      mode === "create"
                        ? "Create conversation"
                        : "Save changes",
                    )}
                  </Button>
                  <Button asChild variant="outline">
                    <Link to={backTo}>
                      <X className="size-4" />
                      {translateText("Cancel")}
                    </Link>
                  </Button>
                </div>
              </form>
            </Form>
          </CardContent>
        </Card>
      )}
    </DetailLayout>
  );
}
