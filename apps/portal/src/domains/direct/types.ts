import type {
  ConversationStatus,
  MessageActorKind,
} from "@/shared/constants/backend-enums";

export type ContactDto = {
  id: string;
  tenantId: string;
  givenName: string;
  surname?: string | null;
  email?: string | null;
  photoUrl?: string | null;
  timeZone?: string | null;
  phoneNumber?: string | null;
  instagramProfileUrl?: string | null;
  tikTokProfileUrl?: string | null;
  facebookProfileUrl?: string | null;
  snapchatProfileUrl?: string | null;
  conversationCount: number;
  createdAtUtc?: string | null;
  lastUpdatedAtUtc?: string | null;
};

export type ContactWriteRequestDto = Omit<
  ContactDto,
  "id" | "tenantId" | "conversationCount" | "createdAtUtc" | "lastUpdatedAtUtc"
>;

export type ConversationDto = {
  id: string;
  tenantId: string;
  contactId: string;
  channelConnectionId: string;
  subject?: string | null;
  status: ConversationStatus;
  messageCount: number;
  lastMessageAtUtc?: string | null;
  createdAtUtc?: string | null;
  lastUpdatedAtUtc?: string | null;
};

export type ConversationDetailDto = ConversationDto & {
  contact: ContactDto;
};

export type ConversationWriteRequestDto = {
  contactId: string;
  channelConnectionId: string;
  subject?: string | null;
  status: ConversationStatus;
};

export type ConversationMessageDto = {
  id: string;
  tenantId: string;
  conversationId: string;
  actorKind: MessageActorKind;
  body: string;
  sentAtUtc: string;
  createdAtUtc?: string | null;
};

export type ConversationMessageCreateRequestDto = {
  conversationId: string;
  actorKind: MessageActorKind;
  body: string;
  sentAtUtc: string;
};
