import { createContext } from "react";
import type { TenantSummaryDto } from "@/domains/tenants/types";
import type { ModuleEnum } from "@/shared/constants/backend-enums";

export type TenantContextValue = {
  tenants: TenantSummaryDto[];
  currentTenantId?: string;
  currentTenant?: TenantSummaryDto;
  currentWorkspaceId?: string;
  isLoading: boolean;
  getModuleTenantId: (module: ModuleEnum) => string | undefined;
  setCurrentTenantId: (tenantId: string) => void;
  refreshTenants: () => Promise<void>;
};

export const TenantContext = createContext<TenantContextValue | undefined>(
  undefined,
);
