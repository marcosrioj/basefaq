import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Save, X } from "lucide-react";
import {
  useChannelConnection,
  useCreateChannelConnection,
  useUpdateChannelConnection,
} from "@/domains/channel-connections/hooks";
import {
  channelConnectionFormSchema,
  type ChannelConnectionFormValues,
} from "@/domains/channel-connections/schemas";
import { settingsNavItems } from "@/domains/settings/settings-nav";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import {
  PageHeader,
  SectionGrid,
  SettingsLayout,
} from "@/shared/layout/page-layouts";
import {
  ChannelConnectionKind,
  backendEnumSelectOptions,
  channelConnectionKindLabels,
} from "@/shared/constants/backend-enums";
import { formatOptionalDateTimeInTimeZone } from "@/shared/lib/time-zone";
import { translateText } from "@/shared/lib/i18n-core";
import {
  Button,
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardHeading,
  CardTitle,
  Form,
  FormCardSkeleton,
  FormSectionHeading,
} from "@/shared/ui";
import {
  SelectField,
  SwitchField,
  TextareaField,
  TextField,
} from "@/shared/ui/form-fields";
import { ErrorState } from "@/shared/ui/placeholder-state";
import {
  ChannelConnectionKindBadge,
  ChannelConnectionStatusBadge,
} from "@/shared/ui/status-badges";
import { useForm } from "react-hook-form";

const kindOptions = backendEnumSelectOptions(channelConnectionKindLabels);

export function ChannelConnectionFormPage({
  mode,
}: {
  mode: "create" | "edit";
}) {
  const { id } = useParams();
  const navigate = useNavigate();
  const timeZone = usePortalTimeZone();
  const connectionQuery = useChannelConnection(
    mode === "edit" ? id : undefined,
  );
  const createConnection = useCreateChannelConnection();
  const updateConnection = useUpdateChannelConnection(id ?? "");
  const form = useForm<ChannelConnectionFormValues>({
    resolver: zodResolver(channelConnectionFormSchema),
    defaultValues: {
      name: "",
      providerKey: "",
      kind: ChannelConnectionKind.WebChat,
      connectionData: "{}",
      isEnabled: true,
    },
  });

  useEffect(() => {
    if (!connectionQuery.data) {
      return;
    }

    form.reset({
      name: connectionQuery.data.name,
      providerKey: connectionQuery.data.providerKey,
      kind: connectionQuery.data.kind,
      connectionData: "",
      isEnabled: connectionQuery.data.isEnabled,
    });
  }, [connectionQuery.data, form]);

  const data = connectionQuery.data;
  const isSubmitting = createConnection.isPending || updateConnection.isPending;
  const backTo = "/app/settings/channel-connections";

  return (
    <SettingsLayout
      currentKey="channel-connections"
      items={settingsNavItems}
      header={
        <PageHeader
          title={
            mode === "create"
              ? "New channel connection"
              : data?.name
                ? `${translateText("Edit")} ${data.name}`
                : "Edit channel connection"
          }
          description="Define the provider identity and encrypted configuration used by module workflows."
          descriptionMode="hint"
          backTo={backTo}
        />
      }
    >
      {connectionQuery.isError ? (
        <ErrorState
          title="Unable to load channel connection"
          error={connectionQuery.error}
          retry={() => void connectionQuery.refetch()}
        />
      ) : mode === "edit" && connectionQuery.isLoading ? (
        <FormCardSkeleton fields={5} />
      ) : (
        <Card>
          <CardHeader>
            <CardHeading>
              <CardTitle>Connection configuration</CardTitle>
              <CardDescription>
                Provider secrets are accepted on write and omitted from every
                read response.
              </CardDescription>
            </CardHeading>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                className="space-y-6"
                onSubmit={form.handleSubmit(async (values) => {
                  const connectionData = values.connectionData.trim();
                  if (mode === "create" && !connectionData) {
                    form.setError("connectionData", {
                      message: "Provider configuration is required.",
                    });
                    return;
                  }

                  if (mode === "create") {
                    await createConnection.mutateAsync({
                      ...values,
                      connectionData,
                    });
                  } else {
                    await updateConnection.mutateAsync({
                      name: values.name,
                      providerKey: values.providerKey,
                      kind: values.kind,
                      isEnabled: values.isEnabled,
                      connectionData: connectionData || undefined,
                    });
                  }

                  navigate(backTo);
                })}
              >
                <FormSectionHeading
                  title="Provider identity"
                  description="Choose a stable internal identity before connecting the provider account."
                />
                <div className="grid gap-5 lg:grid-cols-2">
                  <TextField
                    control={form.control}
                    name="name"
                    label="Connection name"
                    description="Human-readable name used by operators when selecting a channel."
                  />
                  <TextField
                    control={form.control}
                    name="providerKey"
                    label="Provider key"
                    description="Stable provider account or installation key; it must be unique in this workspace."
                  />
                  <SelectField
                    control={form.control}
                    name="kind"
                    label="Channel kind"
                    description="Provider surface represented by this connection and available to module workflows."
                    options={kindOptions}
                  />
                  <SwitchField
                    control={form.control}
                    name="isEnabled"
                    label="Enabled"
                    description="Allows new Direct conversations and Broadcast threads to select this connection."
                    confirmation={false}
                  />
                </div>

                <FormSectionHeading
                  title="Encrypted provider data"
                  description="Submit a JSON object containing the credentials and provider settings required by the adapter."
                />
                <TextareaField
                  control={form.control}
                  name="connectionData"
                  label="Provider configuration"
                  description={
                    mode === "create"
                      ? "Required JSON object. The server encrypts this value before persistence and never returns it."
                      : "Optional JSON object. Leave blank to preserve the current encrypted value, or enter a complete replacement."
                  }
                  placeholder='{"accountId":"...","accessToken":"..."}'
                  rows={10}
                />

                <div className="flex flex-wrap gap-3">
                  <Button type="submit" disabled={isSubmitting}>
                    <Save className="size-4" />
                    {translateText(
                      mode === "create" ? "Create connection" : "Save changes",
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

      {data ? (
        <>
          <SectionGrid
            items={[
              {
                title: "Channel",
                value: <ChannelConnectionKindBadge kind={data.kind} />,
                description: "Provider surface classification",
              },
              {
                title: "Operational status",
                value: <ChannelConnectionStatusBadge status={data.status} />,
                description: "Managed by connection processing",
              },
              {
                title: "Last connected",
                value: formatOptionalDateTimeInTimeZone(
                  data.lastConnectedAtUtc,
                  timeZone,
                  translateText("Never connected"),
                ),
                description: "Most recent successful provider session",
              },
              {
                title: "Last synchronized",
                value: formatOptionalDateTimeInTimeZone(
                  data.lastSynchronizedAtUtc,
                  timeZone,
                  translateText("Never synchronized"),
                ),
                description: "Most recent completed synchronization",
              },
              {
                title: "Credentials expire",
                value: formatOptionalDateTimeInTimeZone(
                  data.credentialsExpireAtUtc,
                  timeZone,
                  translateText("No expiration reported"),
                ),
                description:
                  "Provider credential expiry reported by the adapter",
              },
              {
                title: "Credentials refreshed",
                value: formatOptionalDateTimeInTimeZone(
                  data.lastCredentialsRefreshAtUtc,
                  timeZone,
                  translateText("Never refreshed"),
                ),
                description: "Most recent successful credential rotation",
              },
              {
                title: "Last error",
                value: formatOptionalDateTimeInTimeZone(
                  data.lastErrorAtUtc,
                  timeZone,
                  translateText("No recorded error"),
                ),
                description:
                  data.lastErrorMessage ?? "No provider error message",
              },
              {
                title: "Last updated",
                value: formatOptionalDateTimeInTimeZone(
                  data.lastUpdatedAtUtc,
                  timeZone,
                  translateText("No update"),
                ),
                description: "Most recent configuration or operational update",
              },
            ]}
          />
        </>
      ) : null}
    </SettingsLayout>
  );
}
