import type {
  ChannelConnectionCreateRequestDto,
  ChannelConnectionDto,
  ChannelConnectionUpdateRequestDto,
} from "@/domains/channel-connections/types";
import {
  portalRequest,
  requireAccessToken,
  requireTenantId,
} from "@/platform/api/http-client";
import { toPagedQuery } from "@/shared/lib/pagination";
import type { PagedResultDto } from "@/shared/types/api";
import type {
  ChannelConnectionKind,
  ChannelConnectionStatus,
} from "@/shared/constants/backend-enums";

const path = "/api/tenant/channel-connections";

export function listChannelConnections(
  accessToken: string | undefined,
  tenantId: string | undefined,
  params: {
    page: number;
    pageSize: number;
    sorting?: string;
    searchText?: string;
    kind?: ChannelConnectionKind;
    status?: ChannelConnectionStatus;
    isEnabled?: boolean;
  },
  signal?: AbortSignal,
) {
  return portalRequest<PagedResultDto<ChannelConnectionDto>>({
    service: "tenant",
    path,
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    query: toPagedQuery(params.page, params.pageSize, params.sorting, {
      SearchText: params.searchText,
      Kind: params.kind,
      Status: params.status,
      IsEnabled: params.isEnabled,
    }),
    signal,
  });
}

export function getChannelConnection(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  signal?: AbortSignal,
) {
  return portalRequest<ChannelConnectionDto>({
    service: "tenant",
    path: `${path}/${id}`,
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    signal,
  });
}

export function createChannelConnection(
  accessToken: string | undefined,
  tenantId: string | undefined,
  body: ChannelConnectionCreateRequestDto,
) {
  return portalRequest<string>({
    service: "tenant",
    path,
    method: "POST",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function updateChannelConnection(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
  body: ChannelConnectionUpdateRequestDto,
) {
  return portalRequest<string>({
    service: "tenant",
    path: `${path}/${id}`,
    method: "PUT",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
    body,
  });
}

export function deleteChannelConnection(
  accessToken: string | undefined,
  tenantId: string | undefined,
  id: string,
) {
  return portalRequest<void>({
    service: "tenant",
    path: `${path}/${id}`,
    method: "DELETE",
    accessToken: requireAccessToken(accessToken),
    tenantId: requireTenantId(tenantId),
  });
}
