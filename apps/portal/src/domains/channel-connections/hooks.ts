import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createChannelConnection,
  deleteChannelConnection,
  getChannelConnection,
  listChannelConnections,
  updateChannelConnection,
} from "@/domains/channel-connections/api";
import type {
  ChannelConnectionCreateRequestDto,
  ChannelConnectionUpdateRequestDto,
} from "@/domains/channel-connections/types";
import { useAuth } from "@/platform/auth/use-auth";
import { useTenant } from "@/platform/tenant/use-tenant";
import { translateText } from "@/shared/lib/i18n-core";

const rootKey = ["portal", "channel-connections"] as const;

function tenantKey(tenantId?: string) {
  return [...rootKey, tenantId ?? "none"] as const;
}

export function useChannelConnectionList(
  params: Parameters<typeof listChannelConnections>[2],
) {
  const { session, status } = useAuth();
  const { currentTenantId } = useTenant();

  return useQuery({
    queryKey: [...tenantKey(currentTenantId), "list", params],
    queryFn: ({ signal }) =>
      listChannelConnections(
        session?.accessToken,
        currentTenantId,
        params,
        signal,
      ),
    enabled: status === "ready" && Boolean(currentTenantId),
  });
}

export function useChannelConnection(id?: string) {
  const { session, status } = useAuth();
  const { currentTenantId } = useTenant();

  return useQuery({
    queryKey: [...tenantKey(currentTenantId), "detail", id ?? "unknown"],
    queryFn: ({ signal }) =>
      getChannelConnection(
        session?.accessToken,
        currentTenantId,
        id ?? "",
        signal,
      ),
    enabled: status === "ready" && Boolean(currentTenantId) && Boolean(id),
  });
}

function useInvalidateChannelConnections() {
  const queryClient = useQueryClient();
  const { currentTenantId } = useTenant();

  return () =>
    queryClient.invalidateQueries({ queryKey: tenantKey(currentTenantId) });
}

export function useCreateChannelConnection() {
  const { session } = useAuth();
  const { currentTenantId } = useTenant();
  const invalidate = useInvalidateChannelConnections();

  return useMutation({
    mutationFn: (body: ChannelConnectionCreateRequestDto) =>
      createChannelConnection(session?.accessToken, currentTenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Channel connection created."));
      await invalidate();
    },
  });
}

export function useUpdateChannelConnection(id: string) {
  const { session } = useAuth();
  const { currentTenantId } = useTenant();
  const invalidate = useInvalidateChannelConnections();

  return useMutation({
    mutationFn: (body: ChannelConnectionUpdateRequestDto) =>
      updateChannelConnection(session?.accessToken, currentTenantId, id, body),
    onSuccess: async () => {
      toast.success(translateText("Channel connection updated."));
      await invalidate();
    },
  });
}

export function useDeleteChannelConnection() {
  const { session } = useAuth();
  const { currentTenantId } = useTenant();
  const invalidate = useInvalidateChannelConnections();

  return useMutation({
    mutationFn: (id: string) =>
      deleteChannelConnection(session?.accessToken, currentTenantId, id),
    onSuccess: async () => {
      toast.success(translateText("Channel connection deleted."));
      await invalidate();
    },
  });
}
