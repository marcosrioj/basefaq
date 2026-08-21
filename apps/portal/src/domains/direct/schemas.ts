import { z } from "zod";
import {
  ConversationStatus,
  MessageActorKind,
} from "@/shared/constants/backend-enums";
import { numericEnumSchema } from "@/shared/lib/zod";

const optionalUrl = (maxLength: number) =>
  z
    .string()
    .max(maxLength, "Keep the URL within the backend limit.")
    .refine((value) => !value || URL.canParse(value), "Enter a valid URL.");

export const contactFormSchema = z.object({
  givenName: z.string().min(1, "Given name is required.").max(100),
  surname: z.string().max(100).optional(),
  email: z
    .string()
    .max(200)
    .refine((value) => !value || z.string().email().safeParse(value).success, {
      message: "Enter a valid email address.",
    })
    .optional(),
  photoUrl: optionalUrl(1000).optional(),
  timeZone: z.string().max(100).optional(),
  phoneNumber: z.string().max(200).optional(),
  instagramProfileUrl: optionalUrl(200).optional(),
  tikTokProfileUrl: optionalUrl(200).optional(),
  facebookProfileUrl: optionalUrl(200).optional(),
  snapchatProfileUrl: optionalUrl(200).optional(),
});

export type ContactFormValues = z.infer<typeof contactFormSchema>;

export const conversationFormSchema = z.object({
  contactId: z.string().uuid("Select a contact."),
  channelConnectionId: z.string().uuid("Select a connected channel."),
  subject: z
    .string()
    .max(500, "Keep the subject within 500 characters.")
    .optional(),
  status: numericEnumSchema(ConversationStatus),
});

export type ConversationFormValues = z.infer<typeof conversationFormSchema>;

export const conversationMessageFormSchema = z.object({
  actorKind: numericEnumSchema(MessageActorKind),
  body: z.string().min(1, "Message body is required.").max(12000),
  sentAtLocal: z.string().min(1, "Sent time is required."),
});

export type ConversationMessageFormValues = z.infer<
  typeof conversationMessageFormSchema
>;
