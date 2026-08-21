import { useEffect, useMemo } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Pencil, Plus, RadioTower, Trash2 } from "lucide-react";
import { useChannelConnectionList } from "@/domains/channel-connections/hooks";
import {
  useBroadcastTenantId,
  useBroadcastThreadList,
  useDeleteBroadcastThread,
} from "@/domains/broadcast/hooks";
import type { BroadcastThreadDto } from "@/domains/broadcast/types";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import { ListLayout, PageHeader } from "@/shared/layout/page-layouts";
import {
  BroadcastThreadStatus,
  backendEnumSelectOptions,
  broadcastThreadStatusLabels,
} from "@/shared/constants/backend-enums";
import { translateText } from "@/shared/lib/i18n-core";
import { clampPage } from "@/shared/lib/pagination";
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
import { BroadcastThreadStatusBadge } from "@/shared/ui/status-badges";

const filterDefaults = { status: "", channelId: "" } as const;
const statusOptions = backendEnumSelectOptions(broadcastThreadStatusLabels);

export function BroadcastThreadListPage() {
  const navigate = useNavigate();
  const tenantId = useBroadcastTenantId();
  const timeZone = usePortalTimeZone();
  const state = useListQueryState({
    defaultSorting: "LastItemAtUtc DESC",
    filterDefaults,
  });
  const query = useBroadcastThreadList({
    page: state.page,
    pageSize: state.pageSize,
    sorting: state.sorting,
    searchText: state.debouncedSearch || undefined,
    channelConnectionId: state.filters.channelId || undefined,
    status: state.filters.status
      ? (Number(state.filters.status) as BroadcastThreadStatus)
      : undefined,
  });
  const connectionsQuery = useChannelConnectionList({
    page: 1,
    pageSize: 100,
    sorting: "Name ASC",
  });
  const deleteThread = useDeleteBroadcastThread();
  const connections = useMemo(
    () =>
      new Map(
        (connectionsQuery.data?.items ?? []).map((item) => [
          item.id,
          item.name,
        ]),
      ),
    [connectionsQuery.data?.items],
  );
  const activeFilterCount =
    Number(Boolean(state.search.trim())) +
    Number(Boolean(state.filters.status)) +
    Number(Boolean(state.filters.channelId));

  useEffect(() => {
    if (!query.data) {
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

  const columns: DataTableColumn<BroadcastThreadDto>[] = [
    {
      key: "title",
      header: "Thread",
      cell: (thread) => (
        <div className="flex min-w-0 gap-3">
          <span className="mt-0.5 flex size-9 shrink-0 items-center justify-center rounded-lg border border-fuchsia-500/20 bg-fuchsia-500/[0.06] text-fuchsia-700 dark:text-fuchsia-300">
            <RadioTower className="size-4" />
          </span>
          <div className="min-w-0">
            <div className="break-words font-medium text-mono">
              {thread.title || translateText("Untitled thread")}
            </div>
            <div className="mt-1 text-xs text-muted-foreground">
              {connections.get(thread.channelConnectionId) ??
                translateText("Unknown connection")}
            </div>
          </div>
        </div>
      ),
    },
    {
      key: "status",
      header: "Status",
      className: "xl:w-[110px]",
      cell: (thread) => <BroadcastThreadStatusBadge status={thread.status} />,
    },
    {
      key: "items",
      header: "Items",
      className: "xl:w-[95px]",
      cell: (thread) => (
        <Badge
          appearance="outline"
          variant={thread.itemCount ? "info" : "outline"}
        >
          {thread.itemCount}
        </Badge>
      ),
    },
    {
      key: "lastItem",
      header: "Last capture",
      className: "xl:w-[155px]",
      cell: (thread) => (
        <span className="text-sm text-muted-foreground">
          {formatOptionalDateTimeInTimeZone(
            thread.lastItemAtUtc,
            timeZone,
            translateText("No items"),
          )}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      className: "xl:w-[92px]",
      cell: (thread) => (
        <div
          className="flex items-center gap-1 lg:justify-end"
          onClick={(event) => event.stopPropagation()}
        >
          <Button asChild variant="outline" size="sm" mode="icon">
            <Link to={`/app/broadcast/threads/${thread.id}/edit`}>
              <Pencil className="size-4" />
              <span className="sr-only">{translateText("Edit")}</span>
            </Link>
          </Button>
          <ConfirmAction
            title={translateText("Delete this Broadcast thread?")}
            description={translateText(
              "Threads with captured items cannot be deleted. Close the thread when its public history must be preserved.",
            )}
            confirmLabel={translateText("Delete thread")}
            isPending={deleteThread.isPending}
            onConfirm={() => deleteThread.mutateAsync(thread.id)}
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
    <ListLayout
      header={
        <PageHeader
          title="Broadcast threads"
          description="Coordinate public and community interaction streams by channel and status."
          descriptionMode="hint"
        />
      }
      filters={
        tenantId ? (
          <ListFilterDisclosure
            search={
              <ListFilterSearch
                value={state.search}
                onChange={state.setSearch}
                placeholder="Search Broadcast threads"
                activeFilterCount={activeFilterCount}
                onClear={state.resetFilters}
                isLoading={query.isFetching}
              />
            }
            activeFilterCount={activeFilterCount}
            isLoading={query.isFetching}
          >
            <ListFilterToolbar isLoading={query.isFetching}>
              <ListFilterField label="Status">
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
              <ListFilterField label="Channel">
                <Select
                  value={state.filters.channelId || "all"}
                  onValueChange={(value) =>
                    state.setFilter("channelId", value === "all" ? "" : value)
                  }
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">
                      {translateText("All channels")}
                    </SelectItem>
                    {(connectionsQuery.data?.items ?? []).map((connection) => (
                      <SelectItem key={connection.id} value={connection.id}>
                        {connection.name}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </ListFilterField>
            </ListFilterToolbar>
          </ListFilterDisclosure>
        ) : undefined
      }
    >
      {!tenantId ? (
        <TenantRequiredState />
      ) : (
        <DataTable
          title="Public interaction queue"
          description="Open a row to review its chronological posts, comments, and shared messages."
          descriptionMode="hint"
          columns={columns}
          rows={query.data?.items ?? []}
          getRowId={(thread) => thread.id}
          loading={query.isLoading}
          onRowClick={(thread) =>
            navigate(`/app/broadcast/threads/${thread.id}`)
          }
          toolbar={
            <Button asChild className="ms-auto">
              <Link to="/app/broadcast/threads/new">
                <Plus className="size-4" />
                {translateText("New thread")}
              </Link>
            </Button>
          }
          emptyState={
            <EmptyState
              title="No Broadcast threads in view"
              description="Create a thread or clear the current filters to review other public interactions."
              action={{ label: "New thread", to: "/app/broadcast/threads/new" }}
            />
          }
          errorState={
            query.isError ? (
              <ErrorState
                title="Unable to load Broadcast threads"
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
      )}
    </ListLayout>
  );
}
