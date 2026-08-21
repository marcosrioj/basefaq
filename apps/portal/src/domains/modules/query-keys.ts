export function moduleTenantKey(
  module: "direct" | "broadcast",
  tenantId?: string,
) {
  return ["portal", module, tenantId ?? "none"] as const;
}

export function createModuleDomainKeys(
  module: "direct" | "broadcast",
  domain: string,
) {
  return {
    all: (tenantId?: string) =>
      [...moduleTenantKey(module, tenantId), domain] as const,
    list: (tenantId: string | undefined, params: Record<string, unknown>) =>
      [...moduleTenantKey(module, tenantId), domain, "list", params] as const,
    detail: (tenantId: string | undefined, id: string) =>
      [...moduleTenantKey(module, tenantId), domain, "detail", id] as const,
  };
}
