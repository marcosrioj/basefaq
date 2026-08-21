import type {
  ContactDto,
  ContactWriteRequestDto,
  ConversationDetailDto,
  ConversationDto,
  ConversationMessageCreateRequestDto,
  ConversationMessageDto,
  ConversationWriteRequestDto,
} from "@/domains/direct/types";
import {
  portalRequest,
  requireAccessToken,
  requireTenantId,
} from "@/platform/api/http-client";
import type {
  ConversationStatus,
  MessageActorKind,
} from "@/shared/constants/backend-enums";
import { toPagedQuery } from "@/shared/lib/pagination";
import type { PagedResultDto } from "@/shared/types/api";

export function listContacts(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    page: number;
    pageSize: number;
    sorting?: string;
    searchText?: string;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<ContactDto>>({
    service: "direct",
    path: "/api/direct/contacts",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      SearchText: params.searchText,
    }),
    signal,
  });
}

export function getContact(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  signal?: AbortSignal,
) {
  return portalRequest<ContactDto>({
    service: "direct",
    path: `/api/direct/contacts/${id}`,
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    signal,
  });
}

export function createContact(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: ContactWriteRequestDto,
) {
  return portalRequest<string>({
    service: "direct",
    path: "/api/direct/contacts",
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function updateContact(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  body: ContactWriteRequestDto,
) {
  return portalRequest<string>({
    service: "direct",
    path: `/api/direct/contacts/${id}`,
    method: "PUT",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function deleteContact(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
) {
  return portalRequest<void>({
    service: "direct",
    path: `/api/direct/contacts/${id}`,
    method: "DELETE",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
  });
}

export function listConversations(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    page: number;
    pageSize: number;
    sorting?: string;
    searchText?: string;
    contactId?: string;
    channelConnectionId?: string;
    status?: ConversationStatus;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<ConversationDto>>({
    service: "direct",
    path: "/api/direct/conversations",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      SearchText: params.searchText,
      ContactId: params.contactId,
      ChannelConnectionId: params.channelConnectionId,
      Status: params.status,
    }),
    signal,
  });
}

export function getConversation(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  signal?: AbortSignal,
) {
  return portalRequest<ConversationDetailDto>({
    service: "direct",
    path: `/api/direct/conversations/${id}`,
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    signal,
  });
}

export function createConversation(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: ConversationWriteRequestDto,
) {
  return portalRequest<string>({
    service: "direct",
    path: "/api/direct/conversations",
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function updateConversation(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  body: ConversationWriteRequestDto,
) {
  return portalRequest<string>({
    service: "direct",
    path: `/api/direct/conversations/${id}`,
    method: "PUT",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function deleteConversation(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
) {
  return portalRequest<void>({
    service: "direct",
    path: `/api/direct/conversations/${id}`,
    method: "DELETE",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
  });
}

export function listConversationMessages(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    conversationId: string;
    page: number;
    pageSize: number;
    sorting?: string;
    actorKind?: MessageActorKind;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<ConversationMessageDto>>({
    service: "direct",
    path: "/api/direct/conversation-messages",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      ConversationId: params.conversationId,
      ActorKind: params.actorKind,
    }),
    signal,
  });
}

export function createConversationMessage(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: ConversationMessageCreateRequestDto,
) {
  return portalRequest<string>({
    service: "direct",
    path: "/api/direct/conversation-messages",
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}
