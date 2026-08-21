import { zodResolver } from "@hookform/resolvers/zod";
import { Pencil, Plus, Trash2 } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useChannelConnection } from "@/domains/channel-connections/hooks";
import {
  useBroadcastItemList,
  useBroadcastTenantId,
  useBroadcastThread,
  useCreateBroadcastItem,
  useDeleteBroadcastThread,
} from "@/domains/broadcast/hooks";
import {
  broadcastItemFormSchema,
  type BroadcastItemFormValues,
} from "@/domains/broadcast/schemas";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import {
  DetailLayout,
  KeyValueList,
  PageHeader,
} from "@/shared/layout/page-layouts";
import {
  BroadcastActorKind,
  BroadcastItemKind,
  BroadcastThreadStatus,
  backendEnumSelectOptions,
  broadcastActorKindLabels,
  broadcastItemKindLabels,
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
  BroadcastActorKindBadge,
  BroadcastItemKindBadge,
  BroadcastThreadStatusBadge,
  ChannelConnectionKindBadge,
  ChannelConnectionStatusBadge,
} from "@/shared/ui/status-badges";

const itemKindOptions = backendEnumSelectOptions(broadcastItemKindLabels);
const actorKindOptions = backendEnumSelectOptions(broadcastActorKindLabels);

export function BroadcastThreadDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const tenantId = useBroadcastTenantId();
  const timeZone = usePortalTimeZone();
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const threadQuery = useBroadcastThread(id);
  const itemsQuery = useBroadcastItemList({
    threadId: id ?? "",
    page,
    pageSize,
    sorting: "CapturedAtUtc DESC",
  });
  const connectionQuery = useChannelConnection(
    threadQuery.data?.channelConnectionId,
  );
  const createItem = useCreateBroadcastItem();
  const deleteThread = useDeleteBroadcastThread();
  const form = useForm<BroadcastItemFormValues>({
    resolver: zodResolver(broadcastItemFormSchema),
    defaultValues: {
      kind: BroadcastItemKind.Comment,
      actorKind: BroadcastActorKind.ExternalUser,
      body: "",
      capturedAtLocal: toDateTimeLocalInputValue(),
    },
  });
  const data = threadQuery.data;
  const isOpen = data?.status === BroadcastThreadStatus.Open;

  return (
    <DetailLayout
      header={
        <PageHeader
          title={data?.title || "Broadcast thread"}
          description="Review the complete public interaction timeline and its capture provenance."
          descriptionMode="hint"
          backTo="/app/broadcast/threads"
          actions={
            data ? (
              <>
                <Button asChild variant="outline">
                  <Link to={`/app/broadcast/threads/${data.id}/edit`}>
                    <Pencil className="size-4" />
                    {translateText("Edit")}
                  </Link>
                </Button>
                <ConfirmAction
                  title={translateText("Delete this Broadcast thread?")}
                  description={translateText(
                    "Only threads without captured items can be deleted. Close it instead when public history must be retained.",
                  )}
                  confirmLabel={translateText("Delete thread")}
                  isPending={deleteThread.isPending}
                  onConfirm={async () => {
                    await deleteThread.mutateAsync(data.id);
                    navigate("/app/broadcast/threads");
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
                  <CardTitle>Thread summary</CardTitle>
                </CardHeading>
              </CardHeader>
              <CardContent>
                <KeyValueList
                  items={[
                    {
                      label: "Status",
                      value: (
                        <BroadcastThreadStatusBadge status={data.status} />
                      ),
                    },
                    { label: "Captured items", value: String(data.itemCount) },
                    {
                      label: "Last capture",
                      value: formatOptionalDateTimeInTimeZone(
                        data.lastItemAtUtc,
                        timeZone,
                        translateText("No items"),
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
                    Provider account from which this public stream is captured.
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
          </>
        ) : undefined
      }
    >
      {!tenantId ? (
        <TenantRequiredState />
      ) : threadQuery.isError ? (
        <ErrorState
          title="Unable to load Broadcast thread"
          error={threadQuery.error}
          retry={() => void threadQuery.refetch()}
        />
      ) : (
        <>
          {isOpen ? (
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Add captured item</CardTitle>
                  <CardDescription>
                    Append a provider event to the immutable public timeline.
                  </CardDescription>
                </CardHeading>
              </CardHeader>
              <CardContent>
                <Form {...form}>
                  <form
                    className="space-y-5"
                    onSubmit={form.handleSubmit(async (values) => {
                      await createItem.mutateAsync({
                        threadId: id ?? "",
                        kind: values.kind,
                        actorKind: values.actorKind,
                        body: values.body.trim(),
                        capturedAtUtc: dateTimeLocalInputToUtc(
                          values.capturedAtLocal,
                        ),
                      });
                      form.reset({
                        kind: values.kind,
                        actorKind: values.actorKind,
                        body: "",
                        capturedAtLocal: toDateTimeLocalInputValue(),
                      });
                      setPage(1);
                    })}
                  >
                    <FormSectionHeading
                      title="Capture evidence"
                      description="Classify the public event, its producer, exact body, and provider timestamp."
                    />
                    <div className="grid gap-5 md:grid-cols-3">
                      <SelectField
                        control={form.control}
                        name="kind"
                        label="Item kind"
                        description="Interaction shape: top-level post, comment, shared message, or another supported type."
                        options={itemKindOptions}
                      />
                      <SelectField
                        control={form.control}
                        name="actorKind"
                        label="Actor kind"
                        description="Producer of the item: external audience member, brand, or system."
                        options={actorKindOptions}
                      />
                      <TextField
                        control={form.control}
                        name="capturedAtLocal"
                        type="datetime-local"
                        label="Captured time"
                        description="Local representation of the provider event time; stored as UTC."
                      />
                    </div>
                    <TextareaField
                      control={form.control}
                      name="body"
                      label="Item body"
                      description="Exact text captured from the provider surface, limited to 12,000 characters."
                      rows={5}
                    />
                    <Button type="submit" disabled={createItem.isPending}>
                      <Plus className="size-4" />
                      {translateText("Add captured item")}
                    </Button>
                  </form>
                </Form>
              </CardContent>
            </Card>
          ) : data ? (
            <div className="rounded-lg border border-border bg-muted/25 p-4 text-sm text-muted-foreground">
              {translateText(
                "This Broadcast thread is closed. Reopen it before adding another captured item.",
              )}
            </div>
          ) : null}

          {itemsQuery.isError ? (
            <ErrorState
              title="Unable to load captured items"
              error={itemsQuery.error}
              retry={() => void itemsQuery.refetch()}
            />
          ) : (
            <Card>
              <CardHeader>
                <CardHeading>
                  <CardTitle>Captured timeline</CardTitle>
                  <CardDescription>
                    Newest provider event first. Capture time is distinct from
                    persistence time.
                  </CardDescription>
                </CardHeading>
              </CardHeader>
              <CardContent className="space-y-5">
                {itemsQuery.isLoading ? (
                  <p className="py-8 text-center text-sm text-muted-foreground">
                    {translateText("Loading captured items...")}
                  </p>
                ) : itemsQuery.data?.items.length ? (
                  <div className="divide-y divide-border rounded-lg border border-border">
                    {itemsQuery.data.items.map((item) => (
                      <article key={item.id} className="space-y-3 p-4 sm:p-5">
                        <div className="flex flex-wrap items-center justify-between gap-2">
                          <div className="flex flex-wrap gap-2">
                            <BroadcastItemKindBadge kind={item.kind} />
                            <BroadcastActorKindBadge kind={item.actorKind} />
                          </div>
                          <time className="text-xs text-muted-foreground">
                            {formatOptionalDateTimeInTimeZone(
                              item.capturedAtUtc,
                              timeZone,
                              translateText("Unknown"),
                            )}
                          </time>
                        </div>
                        <p className="whitespace-pre-wrap break-words text-sm leading-6 text-foreground">
                          {item.body}
                        </p>
                      </article>
                    ))}
                  </div>
                ) : (
                  <div className="py-10 text-center">
                    <p className="font-medium text-mono">
                      {translateText("No captured items yet")}
                    </p>
                    <p className="mt-1 text-sm text-muted-foreground">
                      {translateText(
                        "The first provider event will appear here.",
                      )}
                    </p>
                  </div>
                )}
                {itemsQuery.data ? (
                  <PaginationControls
                    page={page}
                    pageSize={pageSize}
                    totalCount={itemsQuery.data.totalCount}
                    onPageChange={setPage}
                    onPageSizeChange={(nextPageSize) => {
                      setPageSize(nextPageSize);
                      setPage(1);
                    }}
                    isFetching={itemsQuery.isFetching}
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
