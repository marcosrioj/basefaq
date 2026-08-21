import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { toast } from "sonner";
import {
  createContact,
  createConversation,
  createConversationMessage,
  deleteContact,
  deleteConversation,
  getContact,
  getConversation,
  listContacts,
  listConversationMessages,
  listConversations,
  updateContact,
  updateConversation,
} from "@/domains/direct/api";
import type {
  ContactWriteRequestDto,
  ConversationMessageCreateRequestDto,
  ConversationWriteRequestDto,
} from "@/domains/direct/types";
import {
  createModuleDomainKeys,
  moduleTenantKey,
} from "@/domains/modules/query-keys";
import { useAuth } from "@/platform/auth/use-auth";
import { useTenant } from "@/platform/tenant/use-tenant";
import { translateText } from "@/shared/lib/i18n-core";

export const contactKeys = createModuleDomainKeys("direct", "contacts");
export const conversationKeys = createModuleDomainKeys(
  "direct",
  "conversations",
);
export const conversationMessageKeys = createModuleDomainKeys(
  "direct",
  "conversation-messages",
);

export function useDirectTenantId() {
  return useTenant().currentTenantId;
}

export function useContactList(
  params: Parameters<typeof listContacts>[2] & { enabled?: boolean },
) {
  const { session, status } = useAuth();
  const tenantId = useDirectTenantId();
  const { enabled = true, ...requestParams } = params;

  return useQuery({
    queryKey: contactKeys.list(tenantId, requestParams),
    queryFn: ({ signal }) =>
      listContacts(session?.accessToken, tenantId, requestParams, signal),
    enabled: enabled && status === "ready" && Boolean(tenantId),
  });
}

export function useContact(id?: string) {
  const { session, status } = useAuth();
  const tenantId = useDirectTenantId();

  return useQuery({
    queryKey: contactKeys.detail(tenantId, id ?? "unknown"),
    queryFn: ({ signal }) =>
      getContact(session?.accessToken, tenantId, id ?? "", signal),
    enabled: status === "ready" && Boolean(tenantId) && Boolean(id),
  });
}

export function useConversationList(
  params: Parameters<typeof listConversations>[2] & { enabled?: boolean },
) {
  const { session, status } = useAuth();
  const tenantId = useDirectTenantId();
  const { enabled = true, ...requestParams } = params;

  return useQuery({
    queryKey: conversationKeys.list(tenantId, requestParams),
    queryFn: ({ signal }) =>
      listConversations(session?.accessToken, tenantId, requestParams, signal),
    enabled: enabled && status === "ready" && Boolean(tenantId),
  });
}

export function useConversation(id?: string) {
  const { session, status } = useAuth();
  const tenantId = useDirectTenantId();

  return useQuery({
    queryKey: conversationKeys.detail(tenantId, id ?? "unknown"),
    queryFn: ({ signal }) =>
      getConversation(session?.accessToken, tenantId, id ?? "", signal),
    enabled: status === "ready" && Boolean(tenantId) && Boolean(id),
  });
}

export function useConversationMessageList(
  params: Parameters<typeof listConversationMessages>[2] & {
    enabled?: boolean;
  },
) {
  const { session, status } = useAuth();
  const tenantId = useDirectTenantId();
  const { enabled = true, ...requestParams } = params;

  return useQuery({
    queryKey: conversationMessageKeys.list(tenantId, requestParams),
    queryFn: ({ signal }) =>
      listConversationMessages(
        session?.accessToken,
        tenantId,
        requestParams,
        signal,
      ),
    enabled:
      enabled &&
      status === "ready" &&
      Boolean(tenantId) &&
      Boolean(requestParams.conversationId),
  });
}

function useDirectMutationContext() {
  const { session } = useAuth();
  const tenantId = useDirectTenantId();
  const queryClient = useQueryClient();

  return {
    accessToken: session?.accessToken,
    tenantId,
    invalidate: () =>
      queryClient.invalidateQueries({
        queryKey: moduleTenantKey("direct", tenantId),
      }),
  };
}

export function useCreateContact() {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (body: ContactWriteRequestDto) =>
      createContact(context.accessToken, context.tenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Contact created."));
      await context.invalidate();
    },
  });
}

export function useUpdateContact(id: string) {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (body: ContactWriteRequestDto) =>
      updateContact(context.accessToken, context.tenantId, id, body),
    onSuccess: async () => {
      toast.success(translateText("Contact updated."));
      await context.invalidate();
    },
  });
}

export function useDeleteContact() {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (id: string) =>
      deleteContact(context.accessToken, context.tenantId, id),
    onSuccess: async () => {
      toast.success(translateText("Contact deleted."));
      await context.invalidate();
    },
  });
}

export function useCreateConversation() {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (body: ConversationWriteRequestDto) =>
      createConversation(context.accessToken, context.tenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Conversation created."));
      await context.invalidate();
    },
  });
}

export function useUpdateConversation(id: string) {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (body: ConversationWriteRequestDto) =>
      updateConversation(context.accessToken, context.tenantId, id, body),
    onSuccess: async () => {
      toast.success(translateText("Conversation updated."));
      await context.invalidate();
    },
  });
}

export function useDeleteConversation() {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (id: string) =>
      deleteConversation(context.accessToken, context.tenantId, id),
    onSuccess: async () => {
      toast.success(translateText("Conversation deleted."));
      await context.invalidate();
    },
  });
}

export function useCreateConversationMessage() {
  const context = useDirectMutationContext();
  return useMutation({
    mutationFn: (body: ConversationMessageCreateRequestDto) =>
      createConversationMessage(context.accessToken, context.tenantId, body),
    onSuccess: async () => {
      toast.success(translateText("Message added."));
      await context.invalidate();
    },
  });
}
