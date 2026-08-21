import { useEffect, useMemo } from "react";
import { Link, useNavigate } from "react-router-dom";
import { MessageSquareText, Pencil, Plus, Trash2 } from "lucide-react";
import { useChannelConnectionList } from "@/domains/channel-connections/hooks";
import {
  useContactList,
  useConversationList,
  useDeleteConversation,
  useDirectTenantId,
} from "@/domains/direct/hooks";
import type { ContactDto, ConversationDto } from "@/domains/direct/types";
import { TenantRequiredState } from "@/domains/modules/tenant-required-state";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import { ListLayout, PageHeader } from "@/shared/layout/page-layouts";
import {
  ConversationStatus,
  backendEnumSelectOptions,
  conversationStatusLabels,
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
import { ConversationStatusBadge } from "@/shared/ui/status-badges";

const filterDefaults = { status: "", contactId: "", channelId: "" } as const;
const statusOptions = backendEnumSelectOptions(conversationStatusLabels);

function contactName(contact?: ContactDto) {
  return contact
    ? [contact.givenName, contact.surname].filter(Boolean).join(" ")
    : undefined;
}

export function ConversationListPage() {
  const navigate = useNavigate();
  const tenantId = useDirectTenantId();
  const timeZone = usePortalTimeZone();
  const state = useListQueryState({
    defaultSorting: "LastMessageAtUtc DESC",
    filterDefaults,
  });
  const query = useConversationList({
    page: state.page,
    pageSize: state.pageSize,
    sorting: state.sorting,
    searchText: state.debouncedSearch || undefined,
    contactId: state.filters.contactId || undefined,
    channelConnectionId: state.filters.channelId || undefined,
    status: state.filters.status
      ? (Number(state.filters.status) as ConversationStatus)
      : undefined,
  });
  const contactsQuery = useContactList({
    page: 1,
    pageSize: 100,
    sorting: "Name ASC",
    enabled: Boolean(tenantId),
  });
  const connectionsQuery = useChannelConnectionList({
    page: 1,
    pageSize: 100,
    sorting: "Name ASC",
  });
  const deleteConversation = useDeleteConversation();
  const contacts = useMemo(
    () =>
      new Map((contactsQuery.data?.items ?? []).map((item) => [item.id, item])),
    [contactsQuery.data?.items],
  );
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
    Number(Boolean(state.filters.contactId)) +
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

  const columns: DataTableColumn<ConversationDto>[] = [
    {
      key: "subject",
      header: "Conversation",
      cell: (conversation) => (
        <div className="flex min-w-0 gap-3">
          <span className="mt-0.5 flex size-9 shrink-0 items-center justify-center rounded-lg border border-blue-500/20 bg-blue-500/[0.06] text-blue-700 dark:text-blue-300">
            <MessageSquareText className="size-4" />
          </span>
          <div className="min-w-0">
            <div className="break-words font-medium text-mono">
              {conversation.subject || translateText("Untitled conversation")}
            </div>
            <div className="mt-1 break-words text-xs text-muted-foreground">
              {contactName(contacts.get(conversation.contactId)) ??
                translateText("Contact {id}", {
                  id: conversation.contactId.slice(0, 8),
                })}
            </div>
          </div>
        </div>
      ),
    },
    {
      key: "status",
      header: "Status",
      className: "xl:w-[110px]",
      cell: (conversation) => (
        <ConversationStatusBadge status={conversation.status} />
      ),
    },
    {
      key: "channel",
      header: "Channel",
      className: "xl:w-[170px]",
      cell: (conversation) => (
        <span className="text-sm text-muted-foreground">
          {connections.get(conversation.channelConnectionId) ??
            translateText("Unknown connection")}
        </span>
      ),
    },
    {
      key: "messages",
      header: "Messages",
      className: "xl:w-[110px]",
      cell: (conversation) => (
        <Badge
          appearance="outline"
          variant={conversation.messageCount ? "primary" : "outline"}
        >
          {conversation.messageCount}
        </Badge>
      ),
    },
    {
      key: "lastMessage",
      header: "Last message",
      className: "xl:w-[145px]",
      cell: (conversation) => (
        <span className="text-sm text-muted-foreground">
          {formatOptionalDateTimeInTimeZone(
            conversation.lastMessageAtUtc,
            timeZone,
            translateText("No messages"),
          )}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      className: "xl:w-[92px]",
      cell: (conversation) => (
        <div
          className="flex items-center gap-1 lg:justify-end"
          onClick={(event) => event.stopPropagation()}
        >
          <Button asChild variant="outline" size="sm" mode="icon">
            <Link to={`/app/direct/conversations/${conversation.id}/edit`}>
              <Pencil className="size-4" />
              <span className="sr-only">{translateText("Edit")}</span>
            </Link>
          </Button>
          <ConfirmAction
            title={translateText("Delete this conversation?")}
            description={translateText(
              "Conversations with messages cannot be deleted. Close the conversation when its history must be preserved.",
            )}
            confirmLabel={translateText("Delete conversation")}
            isPending={deleteConversation.isPending}
            onConfirm={() => deleteConversation.mutateAsync(conversation.id)}
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
          title="Conversations"
          description="Work customer exchanges by status, contact, and connected channel."
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
                placeholder="Search conversations"
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
              <ListFilterField label="Contact">
                <Select
                  value={state.filters.contactId || "all"}
                  onValueChange={(value) =>
                    state.setFilter("contactId", value === "all" ? "" : value)
                  }
                >
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="all">
                      {translateText("All contacts")}
                    </SelectItem>
                    {(contactsQuery.data?.items ?? []).map((contact) => (
                      <SelectItem key={contact.id} value={contact.id}>
                        {contactName(contact)}
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
          title="Conversation queue"
          description="Open a row to review its contact context and chronological message timeline."
          descriptionMode="hint"
          columns={columns}
          rows={query.data?.items ?? []}
          getRowId={(conversation) => conversation.id}
          loading={query.isLoading}
          onRowClick={(conversation) =>
            navigate(`/app/direct/conversations/${conversation.id}`)
          }
          toolbar={
            <Button asChild className="ms-auto">
              <Link to="/app/direct/conversations/new">
                <Plus className="size-4" />
                {translateText("New conversation")}
              </Link>
            </Button>
          }
          emptyState={
            <EmptyState
              title="No conversations in view"
              description="Create a conversation or clear the current filters to review other work."
              action={{
                label: "New conversation",
                to: "/app/direct/conversations/new",
              }}
            />
          }
          errorState={
            query.isError ? (
              <ErrorState
                title="Unable to load conversations"
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
