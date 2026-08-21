import type {
  BroadcastItemCreateRequestDto,
  BroadcastItemDto,
  BroadcastThreadDto,
  BroadcastThreadWriteRequestDto,
} from "@/domains/broadcast/types";
import {
  portalRequest,
  requireAccessToken,
  requireTenantId,
} from "@/platform/api/http-client";
import type {
  BroadcastActorKind,
  BroadcastItemKind,
  BroadcastThreadStatus,
} from "@/shared/constants/backend-enums";
import { toPagedQuery } from "@/shared/lib/pagination";
import type { PagedResultDto } from "@/shared/types/api";

export function listBroadcastThreads(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    page: number;
    pageSize: number;
    sorting?: string;
    searchText?: string;
    channelConnectionId?: string;
    status?: BroadcastThreadStatus;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<BroadcastThreadDto>>({
    service: "broadcast",
    path: "/api/broadcast/threads",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      SearchText: params.searchText,
      ChannelConnectionId: params.channelConnectionId,
      Status: params.status,
    }),
    signal,
  });
}

export function getBroadcastThread(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  signal?: AbortSignal,
) {
  return portalRequest<BroadcastThreadDto>({
    service: "broadcast",
    path: `/api/broadcast/threads/${id}`,
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    signal,
  });
}

export function createBroadcastThread(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: BroadcastThreadWriteRequestDto,
) {
  return portalRequest<string>({
    service: "broadcast",
    path: "/api/broadcast/threads",
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function updateBroadcastThread(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  body: BroadcastThreadWriteRequestDto,
) {
  return portalRequest<string>({
    service: "broadcast",
    path: `/api/broadcast/threads/${id}`,
    method: "PUT",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function deleteBroadcastThread(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
) {
  return portalRequest<void>({
    service: "broadcast",
    path: `/api/broadcast/threads/${id}`,
    method: "DELETE",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
  });
}

export function listBroadcastItems(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    threadId: string;
    page: number;
    pageSize: number;
    sorting?: string;
    kind?: BroadcastItemKind;
    actorKind?: BroadcastActorKind;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<BroadcastItemDto>>({
    service: "broadcast",
    path: "/api/broadcast/items",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      ThreadId: params.threadId,
      Kind: params.kind,
      ActorKind: params.actorKind,
    }),
    signal,
  });
}

export function createBroadcastItem(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: BroadcastItemCreateRequestDto,
) {
  return portalRequest<string>({
    service: "broadcast",
    path: "/api/broadcast/items",
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}
