import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createBroadcastItem,
  createBroadcastThread,
  deleteBroadcastThread,
  getBroadcastThread,
  listBroadcastItems,
  listBroadcastThreads,
  updateBroadcastThread,
} from "@/domains/broadcast/api";
import type {
  BroadcastItemCreateRequestDto,
  BroadcastThreadWriteRequestDto,
} from "@/domains/broadcast/types";
import {
  createModuleDomainKeys,
  moduleTenantKey,
} from "@/domains/modules/query-keys";
import { useAuth } from "@/platform/auth/use-auth";
import { useTenant } from "@/platform/tenant/use-tenant";
import { ModuleEnum } from "@/shared/constants/backend-enums";
import { translateText } from "@/shared/lib/i18n-core";

export const broadcastThreadKeys = createModuleDomainKeys(
  "broadcast",
  "threads",
);
export const broadcastItemKeys = createModuleDomainKeys("broadcast", "items");

export function useBroadcastTenantId() {
  return useTenant().getModuleTenantId(ModuleEnum.Broadcast);
}

export function useBroadcastThreadList(
  params: Parameters<typeof listBroadcastThreads>[2] & { enabled?: boolean },
) {
  const { session, status } = useAuth();
  const tenantId = useBroadcastTenantId();
  const { enabled = true, ...requestParams } = params;

  return useQuery({
    queryKey: broadcastThreadKeys.list(tenantId, requestParams),
    queryFn: ({ signal }) =>
      listBroadcastThreads(
        session?.accessToken,
        tenantId,
        requestParams,
        signal,
      ),
    enabled: enabled && status === "ready" && Boolean(tenantId),
  });
}

export function useBroadcastThread(id?: string) {
  const { session, status } = useAuth();
  const tenantId = useBroadcastTenantId();

  return useQuery({
    queryKey: broadcastThreadKeys.detail(tenantId, id ?? "unknown"),
    queryFn: ({ signal }) =>
      getBroadcastThread(session?.accessToken, tenantId, id ?? "", signal),
    enabled: status === "ready" && Boolean(tenantId) && Boolean(id),
  });
}

export function useBroadcastItemList(
  params: Parameters<typeof listBroadcastItems>[2] & { enabled?: boolean },
) {
  const { session, status } = useAuth();
  const tenantId = useBroadcastTenantId();
  const { enabled = true, ...requestParams } = params;

  return useQuery({
    queryKey: broadcastItemKeys.list(tenantId, requestParams),
    queryFn: ({ signal }) =>
      listBroadcastItems(session?.accessToken, tenantId, requestParams, signal),
    enabled:
      enabled &&
      status === "ready" &&
      Boolean(tenantId) &&
      Boolean(requestParams.threadId),
  });
}

function useBroadcastMutationContext() {
  const { session } = useAuth();
  const tenantId = useBroadcastTenantId();
  const queryClient = useQueryClient();
  return {
    accessToken: session?.accessToken,
    tenantId,
    invalidate: () =>
      queryClient.invalidateQueries({
        queryKey: moduleTenantKey("broadcast", tenantId),
      }),
  };
}

export function useCreateBroadcastThread() {
  const context = useBroadcastMutationContext();
  return useMutation({
    mutationFn: (body: BroadcastThreadWriteRequestDto) =>
      createBroadcastThread(context.accessToken, context.tenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Broadcast thread created."));
      await context.invalidate();
    },
  });
}

export function useUpdateBroadcastThread(id: string) {
  const context = useBroadcastMutationContext();
  return useMutation({
    mutationFn: (body: BroadcastThreadWriteRequestDto) =>
      updateBroadcastThread(context.accessToken, context.tenantId, id, body),
    onSuccess: async () => {
      toast.success(translateText("Broadcast thread updated."));
      await context.invalidate();
    },
  });
}

export function useDeleteBroadcastThread() {
  const context = useBroadcastMutationContext();
  return useMutation({
    mutationFn: (id: string) =>
      deleteBroadcastThread(context.accessToken, context.tenantId, id),
    onSuccess: async () => {
      toast.success(translateText("Broadcast thread deleted."));
      await context.invalidate();
    },
  });
}

export function useCreateBroadcastItem() {
  const context = useBroadcastMutationContext();
  return useMutation({
    mutationFn: (body: BroadcastItemCreateRequestDto) =>
      createBroadcastItem(context.accessToken, context.tenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Broadcast item added."));
      await context.invalidate();
    },
  });
}
