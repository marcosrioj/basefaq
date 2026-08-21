import { z } from "zod";
import {
  BroadcastActorKind,
  BroadcastItemKind,
  BroadcastThreadStatus,
} from "@/shared/constants/backend-enums";
import { numericEnumSchema } from "@/shared/lib/zod";

export const broadcastThreadFormSchema = z.object({
  channelConnectionId: z.string().uuid("Select a connected channel."),
  title: z
    .string()
    .max(1000, "Keep the title within 1,000 characters.")
    .optional(),
  status: numericEnumSchema(BroadcastThreadStatus),
});

export type BroadcastThreadFormValues = z.infer<
  typeof broadcastThreadFormSchema
>;

export const broadcastItemFormSchema = z.object({
  kind: numericEnumSchema(BroadcastItemKind),
  actorKind: numericEnumSchema(BroadcastActorKind),
  body: z.string().min(1, "Item body is required.").max(12000),
  capturedAtLocal: z.string().min(1, "Capture time is required."),
});

export type BroadcastItemFormValues = z.infer<typeof broadcastItemFormSchema>;
