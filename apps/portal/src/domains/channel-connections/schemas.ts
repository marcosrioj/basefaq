import { z } from "zod";
import { ChannelConnectionKind } from "@/shared/constants/backend-enums";
import { numericEnumSchema } from "@/shared/lib/zod";

function isJsonObject(value: string) {
  try {
    const parsed = JSON.parse(value) as unknown;
    return (
      typeof parsed === "object" && parsed !== null && !Array.isArray(parsed)
    );
  } catch {
    return false;
  }
}

export const channelConnectionFormSchema = z.object({
  name: z.string().min(2, "Connection name is required.").max(120),
  providerKey: z.string().min(2, "Provider key is required.").max(200),
  kind: numericEnumSchema(ChannelConnectionKind),
  connectionData: z
    .string()
    .max(16000, "Keep the provider configuration within 16,000 characters.")
    .refine((value) => !value.trim() || isJsonObject(value), {
      message: "Enter a valid JSON object.",
    }),
  isEnabled: z.boolean(),
});

export type ChannelConnectionFormValues = z.infer<
  typeof channelConnectionFormSchema
>;
