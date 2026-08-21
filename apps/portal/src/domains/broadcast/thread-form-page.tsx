import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect, useMemo } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Cable, Save, X } from "lucide-react";
import {
  useChannelConnection,
  useChannelConnectionList,
} from "@/domains/channel-connections/hooks";
import {
  useBroadcastTenantId,
  useBroadcastThread,
  useCreateBroadcastThread,
  useUpdateBroadcastThread,
} from "@/domains/broadcast/hooks";
import {
  broadcastThreadFormSchema,
  type BroadcastThreadFormValues,
} from "@/domains/broadcast/schemas";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import {
  DetailLayout,
  KeyValueList,
  PageHeader,
} from "@/shared/layout/page-layouts";
import {
  BroadcastThreadStatus,
  ChannelConnectionStatus,
  backendEnumSelectOptions,
  broadcastThreadStatusLabels,
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
import { BroadcastThreadStatusBadge } from "@/shared/ui/status-badges";

const statusOptions = backendEnumSelectOptions(broadcastThreadStatusLabels);

export function BroadcastThreadFormPage({ mode }: { mode: "create" | "edit" }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const tenantId = useBroadcastTenantId();
  const threadQuery = useBroadcastThread(mode === "edit" ? id : undefined);
  const connectionsQuery = useChannelConnectionList({
    page: 1,
    pageSize: 100,
    sorting: "Name ASC",
    status: ChannelConnectionStatus.Connected,
    isEnabled: true,
  });
  const currentConnectionQuery = useChannelConnection(
    threadQuery.data?.channelConnectionId,
  );
  const createThread = useCreateBroadcastThread();
  const updateThread = useUpdateBroadcastThread(id ?? "");
  const form = useForm<BroadcastThreadFormValues>({
    resolver: zodResolver(broadcastThreadFormSchema),
    defaultValues: {
      channelConnectionId: "",
      title: "",
      status: BroadcastThreadStatus.Open,
    },
  });

  useEffect(() => {
    if (!threadQuery.data) {
      return;
    }
    form.reset({
      channelConnectionId: threadQuery.data.channelConnectionId,
      title: threadQuery.data.title ?? "",
      status: threadQuery.data.status,
    });
  }, [form, threadQuery.data]);

  const selectedConnectionId = form.watch("channelConnectionId");
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
  const data = threadQuery.data;
  const backTo = id ? `/app/broadcast/threads/${id}` : "/app/broadcast/threads";

  return (
    <DetailLayout
      header={
        <PageHeader
          title={
            mode === "create"
              ? "New Broadcast thread"
              : data?.title
                ? `${translateText("Edit")} ${data.title}`
                : "Edit Broadcast thread"
          }
          description="Anchor a public interaction stream to one connected provider channel."
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
                <CardTitle>Thread summary</CardTitle>
              </CardHeading>
            </CardHeader>
            <CardContent>
              <KeyValueList
                items={[
                  {
                    label: "Status",
                    value: <BroadcastThreadStatusBadge status={data.status} />,
                  },
                  { label: "Captured items", value: String(data.itemCount) },
                  {
                    label: "Channel",
                    value: currentConnectionQuery.data?.name ?? "Unavailable",
                  },
                  { label: "Thread ID", value: data.id },
                ]}
              />
            </CardContent>
          </Card>
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
      ) : mode === "edit" && threadQuery.isLoading ? (
        <FormCardSkeleton fields={3} />
      ) : (
        <Card>
          <CardHeader>
            <CardHeading>
              <CardTitle>Thread details</CardTitle>
            </CardHeading>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                className="space-y-6"
                onSubmit={form.handleSubmit(async (values) => {
                  const body = {
                    ...values,
                    title: values.title?.trim() || undefined,
                  };
                  const threadId =
                    mode === "create"
                      ? await createThread.mutateAsync(body)
                      : await updateThread.mutateAsync(body);
                  navigate(`/app/broadcast/threads/${threadId}`);
                })}
              >
                <FormSectionHeading
                  title="Capture routing"
                  description="Choose the connected public surface that owns this interaction stream."
                />
                <SearchSelectField
                  control={form.control}
                  name="channelConnectionId"
                  label="Channel connection"
                  description="Connected and enabled provider account from which public items are captured."
                  options={connectionOptions}
                  selectedOption={selectedConnectionOption}
                  placeholder="Select a connected channel"
                  searchPlaceholder="Search channel connections"
                  emptyMessage="No connected channels found"
                  loading={connectionsQuery.isFetching}
                />

                <FormSectionHeading
                  title="Coordination state"
                  description="Give operators a recognizable label and control whether new items can be appended."
                />
                <TextareaField
                  control={form.control}
                  name="title"
                  label="Thread title"
                  description="Optional provider topic, post caption, or operator summary; limited to 1,000 characters."
                  rows={4}
                />
                <div className="max-w-md">
                  <SelectField
                    control={form.control}
                    name="status"
                    label="Thread status"
                    description="Open threads accept captured items; closed threads preserve a read-only public timeline."
                    options={statusOptions}
                    confirmation={{
                      title: "Change thread status?",
                      description:
                        "Closing makes the captured timeline read-only. Reopen only when public monitoring resumes.",
                      confirmLabel: "Change status",
                    }}
                  />
                </div>

                <div className="flex flex-wrap gap-3">
                  <Button
                    type="submit"
                    disabled={createThread.isPending || updateThread.isPending}
                  >
                    <Save className="size-4" />
                    {translateText(
                      mode === "create" ? "Create thread" : "Save changes",
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
