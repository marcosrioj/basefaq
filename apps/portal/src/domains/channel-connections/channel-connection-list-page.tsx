import { useEffect } from "react";
import { Cable, Pencil, Plus, Trash2 } from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import {
  useChannelConnectionList,
  useDeleteChannelConnection,
} from "@/domains/channel-connections/hooks";
import type { ChannelConnectionDto } from "@/domains/channel-connections/types";
import { settingsNavItems } from "@/domains/settings/settings-nav";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import { PageHeader, SettingsLayout } from "@/shared/layout/page-layouts";
import {
  ChannelConnectionKind,
  ChannelConnectionStatus,
  backendEnumSelectOptions,
  channelConnectionKindLabels,
  channelConnectionStatusLabels,
} from "@/shared/constants/backend-enums";
import { clampPage } from "@/shared/lib/pagination";
import { translateText } from "@/shared/lib/i18n-core";
import { formatOptionalDateTimeInTimeZone } from "@/shared/lib/time-zone";
import { useListQueryState } from "@/shared/lib/use-list-query-state";
import {
  Badge,
  Button,
  ConfirmAction,
  ListFilterDisclosure,
  ListFilterField,
  ListFilterSearch,
  ListFilterToolbar,
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui";
import { DataTable, type DataTableColumn } from "@/shared/ui/data-table";
import { PaginationControls } from "@/shared/ui/pagination-controls";
import { EmptyState, ErrorState } from "@/shared/ui/placeholder-state";
import {
  ChannelConnectionKindBadge,
  ChannelConnectionStatusBadge,
} from "@/shared/ui/status-badges";

const filterDefaults = { kind: "", status: "", enabled: "" } as const;
const kindOptions = backendEnumSelectOptions(channelConnectionKindLabels);
const statusOptions = backendEnumSelectOptions(channelConnectionStatusLabels);

export function ChannelConnectionListPage() {
  const navigate = useNavigate();
  const timeZone = usePortalTimeZone();
  const state = useListQueryState({
    defaultSorting: "LastUpdatedAtUtc DESC",
    filterDefaults,
  });
  const query = useChannelConnectionList({
    page: state.page,
    pageSize: state.pageSize,
    sorting: state.sorting,
    searchText: state.debouncedSearch || undefined,
    kind: state.filters.kind
      ? (Number(state.filters.kind) as ChannelConnectionKind)
      : undefined,
    status: state.filters.status
      ? (Number(state.filters.status) as ChannelConnectionStatus)
      : undefined,
    isEnabled:
      state.filters.enabled === "enabled"
        ? true
        : state.filters.enabled === "disabled"
          ? false
          : undefined,
  });
  const deleteConnection = useDeleteChannelConnection();
  const activeFilterCount =
    Number(Boolean(state.search.trim())) +
    Number(Boolean(state.filters.kind)) +
    Number(Boolean(state.filters.status)) +
    Number(Boolean(state.filters.enabled));

  useEffect(() => {
    if (query.data === undefined) {
      return;
    }

    const nextPage = clampPage(
      state.page,
      query.data.totalCount,
      state.pageSize,
    );
    if (nextPage !== state.page) {
      state.setPage(nextPage, { replace: true });
    }
  }, [query.data, state]);

  const columns: DataTableColumn<ChannelConnectionDto>[] = [
    {
      key: "name",
      header: "Connection",
      cell: (connection) => (
        <div className="flex min-w-0 gap-3">
          <span className="mt-0.5 flex size-9 shrink-0 items-center justify-center rounded-lg border border-cyan-500/20 bg-cyan-500/[0.06] text-cyan-700 dark:text-cyan-300">
            <Cable className="size-4" />
          </span>
          <div className="min-w-0">
            <div className="break-words font-medium text-mono">
              {connection.name}
            </div>
            <div className="mt-1 break-all text-xs text-muted-foreground">
              {connection.providerKey}
            </div>
          </div>
        </div>
      ),
    },
    {
      key: "kind",
      header: "Channel",
      className: "xl:w-[150px]",
      cell: (connection) => (
        <ChannelConnectionKindBadge kind={connection.kind} />
      ),
    },
    {
      key: "status",
      header: "Health",
      className: "xl:w-[170px]",
      cell: (connection) => (
        <div className="flex flex-wrap gap-2">
          <ChannelConnectionStatusBadge status={connection.status} />
          <Badge
            appearance="outline"
            variant={connection.isEnabled ? "success" : "outline"}
          >
            {translateText(connection.isEnabled ? "Enabled" : "Disabled")}
          </Badge>
        </div>
      ),
    },
    {
      key: "lastSynchronizedAtUtc",
      header: "Last sync",
      className: "xl:w-[145px]",
      cell: (connection) => (
        <span className="text-sm text-muted-foreground">
          {formatOptionalDateTimeInTimeZone(
            connection.lastSynchronizedAtUtc,
            timeZone,
            translateText("Never synchronized"),
          )}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      className: "xl:w-[92px]",
      cell: (connection) => (
        <div
          className="flex items-center gap-1 lg:justify-end"
          onClick={(event) => event.stopPropagation()}
        >
          <Button asChild variant="outline" size="sm" mode="icon">
            <Link
              to={`/app/settings/channel-connections/${connection.id}/edit`}
            >
              <Pencil className="size-4" />
              <span className="sr-only">{translateText("Edit")}</span>
            </Link>
          </Button>
          <ConfirmAction
            title={translateText('Delete connection "{name}"?', {
              name: connection.name,
            })}
            description={translateText(
              "Direct and Broadcast records keep their channel reference. Delete only connections that are no longer in use.",
            )}
            confirmLabel={translateText("Delete connection")}
            isPending={deleteConnection.isPending}
            onConfirm={() => deleteConnection.mutateAsync(connection.id)}
            trigger={
              <Button variant="ghost" size="sm" mode="icon">
                <Trash2 className="size-4 text-destructive" />
                <span className="sr-only">{translateText("Delete")}</span>
              </Button>
            }
          />
        </div>
      ),
    },
  ];

  return (
    <SettingsLayout
      currentKey="channel-connections"
      items={settingsNavItems}
      header={
        <PageHeader
          title="Channel connections"
          description="Manage the provider connections shared by Direct and Broadcast workflows."
          descriptionMode="hint"
        />
      }
    >
      <ListFilterDisclosure
        search={
          <ListFilterSearch
            value={state.search}
            onChange={state.setSearch}
            placeholder="Search connections"
            activeFilterCount={activeFilterCount}
            onClear={state.resetFilters}
            isLoading={query.isFetching}
          />
        }
        activeFilterCount={activeFilterCount}
        isLoading={query.isFetching}
      >
        <ListFilterToolbar isLoading={query.isFetching}>
          <ListFilterField label="Channel kind">
            <Select
              value={state.filters.kind || "all"}
              onValueChange={(value) =>
                state.setFilter("kind", value === "all" ? "" : value)
              }
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {translateText("All channels")}
                </SelectItem>
                {kindOptions.map((option) => (
                  <SelectItem key={option.value} value={option.value}>
                    {translateText(option.label)}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </ListFilterField>
          <ListFilterField label="Connection status">
            <Select
              value={state.filters.status || "all"}
              onValueChange={(value) =>
                state.setFilter("status", value === "all" ? "" : value)
              }
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {translateText("All statuses")}
                </SelectItem>
                {statusOptions.map((option) => (
                  <SelectItem key={option.value} value={option.value}>
                    {translateText(option.label)}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </ListFilterField>
          <ListFilterField label="Availability">
            <Select
              value={state.filters.enabled || "all"}
              onValueChange={(value) =>
                state.setFilter("enabled", value === "all" ? "" : value)
              }
            >
              <SelectTrigger>
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">
                  {translateText("All availability")}
                </SelectItem>
                <SelectItem value="enabled">
                  {translateText("Enabled")}
                </SelectItem>
                <SelectItem value="disabled">
                  {translateText("Disabled")}
                </SelectItem>
              </SelectContent>
            </Select>
          </ListFilterField>
        </ListFilterToolbar>
      </ListFilterDisclosure>

      <DataTable
        title="Provider connections"
        description="Credentials are encrypted at rest and are never returned by the API."
        descriptionMode="hint"
        columns={columns}
        rows={query.data?.items ?? []}
        getRowId={(connection) => connection.id}
        loading={query.isLoading}
        onRowClick={(connection) =>
          navigate(`/app/settings/channel-connections/${connection.id}/edit`)
        }
        toolbar={
          <Button asChild className="ms-auto">
            <Link to="/app/settings/channel-connections/new">
              <Plus className="size-4" />
              {translateText("New connection")}
            </Link>
          </Button>
        }
        emptyState={
          <EmptyState
            title="No channel connections"
            description="Create a provider connection before routing Direct conversations or Broadcast threads."
            action={{
              label: "New connection",
              to: "/app/settings/channel-connections/new",
            }}
          />
        }
        errorState={
          query.isError ? (
            <ErrorState
              title="Unable to load channel connections"
              error={query.error}
              retry={() => void query.refetch()}
            />
          ) : undefined
        }
        footer={
          query.data ? (
            <PaginationControls
              page={state.page}
              pageSize={state.pageSize}
              totalCount={query.data.totalCount}
              onPageChange={state.setPage}
              onPageSizeChange={state.setPageSize}
              isFetching={query.isFetching}
            />
          ) : undefined
        }
      />
    </SettingsLayout>
  );
}
