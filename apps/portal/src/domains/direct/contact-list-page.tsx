import { useEffect } from "react";
import {
  ContactRound,
  MessageSquare,
  Pencil,
  Plus,
  Trash2,
} from "lucide-react";
import { Link, useNavigate } from "react-router-dom";
import {
  useContactList,
  useDeleteContact,
  useDirectTenantId,
} from "@/domains/direct/hooks";
import type { ContactDto } from "@/domains/direct/types";
import { ModuleUnavailableState } from "@/domains/modules/module-unavailable-state";
import { usePortalTimeZone } from "@/domains/settings/settings-hooks";
import { ListLayout, PageHeader } from "@/shared/layout/page-layouts";
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

const sortingOptions = [
  { value: "LastUpdatedAtUtc DESC", label: "Last update newest" },
  { value: "LastUpdatedAtUtc ASC", label: "Last update oldest" },
  { value: "Name ASC", label: "Name A-Z" },
  { value: "Name DESC", label: "Name Z-A" },
  { value: "ConversationCount DESC", label: "Conversations high-low" },
  { value: "ConversationCount ASC", label: "Conversations low-high" },
];

function contactName(contact: ContactDto) {
  return [contact.givenName, contact.surname].filter(Boolean).join(" ");
}

export function ContactListPage() {
  const navigate = useNavigate();
  const tenantId = useDirectTenantId();
  const timeZone = usePortalTimeZone();
  const state = useListQueryState({
    defaultSorting: "LastUpdatedAtUtc DESC",
  });
  const query = useContactList({
    page: state.page,
    pageSize: state.pageSize,
    sorting: state.sorting,
    searchText: state.debouncedSearch || undefined,
  });
  const deleteContact = useDeleteContact();

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

  const columns: DataTableColumn<ContactDto>[] = [
    {
      key: "name",
      header: "Contact",
      cell: (contact) => (
        <div className="flex min-w-0 gap-3">
          <span className="mt-0.5 flex size-9 shrink-0 items-center justify-center rounded-lg border border-emerald-500/20 bg-emerald-500/[0.06] text-emerald-700 dark:text-emerald-300">
            <ContactRound className="size-4" />
          </span>
          <div className="min-w-0">
            <div className="break-words font-medium text-mono">
              {contactName(contact)}
            </div>
            <div className="mt-1 break-all text-xs text-muted-foreground">
              {contact.email ??
                contact.phoneNumber ??
                translateText("No contact method")}
            </div>
          </div>
        </div>
      ),
    },
    {
      key: "location",
      header: "Context",
      className: "xl:w-[190px]",
      cell: (contact) => (
        <div className="space-y-1 text-sm text-muted-foreground">
          <div>{contact.timeZone ?? translateText("No time zone")}</div>
          {contact.phoneNumber ? <div>{contact.phoneNumber}</div> : null}
        </div>
      ),
    },
    {
      key: "conversationCount",
      header: "Conversations",
      className: "xl:w-[145px]",
      cell: (contact) => (
        <Badge
          appearance="outline"
          variant={contact.conversationCount ? "primary" : "outline"}
        >
          {translateText("{count} conversations", {
            count: contact.conversationCount,
          })}
        </Badge>
      ),
    },
    {
      key: "lastUpdatedAtUtc",
      header: "Last update",
      className: "xl:w-[145px]",
      cell: (contact) => (
        <span className="text-sm text-muted-foreground">
          {formatOptionalDateTimeInTimeZone(
            contact.lastUpdatedAtUtc,
            timeZone,
            translateText("No update"),
          )}
        </span>
      ),
    },
    {
      key: "actions",
      header: "Actions",
      className: "xl:w-[126px]",
      cell: (contact) => (
        <div
          className="flex items-center gap-1 lg:justify-end"
          onClick={(event) => event.stopPropagation()}
        >
          <Button asChild variant="outline" size="sm" mode="icon">
            <Link to={`/app/direct/conversations?contactId=${contact.id}`}>
              <MessageSquare className="size-4" />
              <span className="sr-only">
                {translateText("View conversations")}
              </span>
            </Link>
          </Button>
          <Button asChild variant="outline" size="sm" mode="icon">
            <Link to={`/app/direct/contacts/${contact.id}/edit`}>
              <Pencil className="size-4" />
              <span className="sr-only">{translateText("Edit")}</span>
            </Link>
          </Button>
          <ConfirmAction
            title={translateText('Delete contact "{name}"?', {
              name: contactName(contact),
            })}
            description={translateText(
              "Contacts with conversation history cannot be deleted. Preserve the record when its history is still required.",
            )}
            confirmLabel={translateText("Delete contact")}
            isPending={deleteContact.isPending}
            onConfirm={() => deleteContact.mutateAsync(contact.id)}
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
          title="Contacts"
          description="Maintain the customer identities reused across Direct conversations."
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
                placeholder="Search contacts"
                activeFilterCount={state.search.trim() ? 1 : 0}
                onClear={state.resetFilters}
                isLoading={query.isFetching}
              />
            }
            activeFilterCount={0}
            isLoading={query.isFetching}
          >
            <ListFilterToolbar isLoading={query.isFetching}>
              <ListFilterField label="Sort">
                <Select value={state.sorting} onValueChange={state.setSorting}>
                  <SelectTrigger>
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    {sortingOptions.map((option) => (
                      <SelectItem key={option.value} value={option.value}>
                        {translateText(option.label)}
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
        <ModuleUnavailableState module="Direct" />
      ) : (
        <DataTable
          title="Contact directory"
          description="Profile links and customer context stay attached to one reusable identity."
          descriptionMode="hint"
          columns={columns}
          rows={query.data?.items ?? []}
          getRowId={(contact) => contact.id}
          loading={query.isLoading}
          onRowClick={(contact) =>
            navigate(`/app/direct/contacts/${contact.id}/edit`)
          }
          toolbar={
            <Button asChild className="ms-auto">
              <Link to="/app/direct/contacts/new">
                <Plus className="size-4" />
                {translateText("New contact")}
              </Link>
            </Button>
          }
          emptyState={
            <EmptyState
              title="No contacts"
              description="Create the first customer identity before opening a Direct conversation."
              action={{ label: "New contact", to: "/app/direct/contacts/new" }}
            />
          }
          errorState={
            query.isError ? (
              <ErrorState
                title="Unable to load contacts"
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
