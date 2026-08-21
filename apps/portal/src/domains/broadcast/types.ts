import type {
  BroadcastActorKind,
  BroadcastItemKind,
  BroadcastThreadStatus,
} from "@/shared/constants/backend-enums";

export type BroadcastThreadDto = {
  id: string;
  tenantId: string;
  channelConnectionId: string;
  title?: string | null;
  status: BroadcastThreadStatus;
  itemCount: number;
  lastItemAtUtc?: string | null;
  createdAtUtc?: string | null;
  lastUpdatedAtUtc?: string | null;
};

export type BroadcastThreadWriteRequestDto = {
  channelConnectionId: string;
  title?: string | null;
  status: BroadcastThreadStatus;
};

export type BroadcastItemDto = {
  id: string;
  tenantId: string;
  threadId: string;
  kind: BroadcastItemKind;
  actorKind: BroadcastActorKind;
  body: string;
  capturedAtUtc: string;
  createdAtUtc?: string | null;
};

export type BroadcastItemCreateRequestDto = {
  threadId: string;
  kind: BroadcastItemKind;
  actorKind: BroadcastActorKind;
  body: string;
  capturedAtUtc: string;
};
