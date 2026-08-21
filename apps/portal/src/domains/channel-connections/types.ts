import type {
  ChannelConnectionKind,
  ChannelConnectionStatus,
} from "@/shared/constants/backend-enums";

export type ChannelConnectionDto = {
  id: string;
  tenantId: string;
  name: string;
  providerKey: string;
  kind: ChannelConnectionKind;
  status: ChannelConnectionStatus;
  isEnabled: boolean;
  credentialsExpireAtUtc?: string | null;
  lastCredentialsRefreshAtUtc?: string | null;
  lastConnectedAtUtc?: string | null;
  lastSynchronizedAtUtc?: string | null;
  lastErrorAtUtc?: string | null;
  lastErrorMessage?: string | null;
  createdAtUtc?: string | null;
  lastUpdatedAtUtc?: string | null;
};

export type ChannelConnectionCreateRequestDto = {
  name: string;
  providerKey: string;
  kind: ChannelConnectionKind;
  connectionData: string;
  isEnabled: boolean;
};

export type ChannelConnectionUpdateRequestDto = Omit<
  ChannelConnectionCreateRequestDto,
  "connectionData"
> & {
  connectionData?: string;
};
