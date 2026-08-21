import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Save, X } from "lucide-react";
import {
  useContact,
  useCreateContact,
  useDirectTenantId,
  useUpdateContact,
} from "@/domains/direct/hooks";
import {
  contactFormSchema,
  type ContactFormValues,
} from "@/domains/direct/schemas";
import { ModuleUnavailableState } from "@/domains/modules/module-unavailable-state";
import {
  DetailLayout,
  KeyValueList,
  PageHeader,
} from "@/shared/layout/page-layouts";
import { translateText } from "@/shared/lib/i18n-core";
import {
  Button,
  Card,
  CardContent,
  CardHeader,
  CardHeading,
  CardTitle,
  Form,
  FormCardSkeleton,
  FormSectionHeading,
} from "@/shared/ui";
import { TextField } from "@/shared/ui/form-fields";
import { ErrorState } from "@/shared/ui/placeholder-state";

const emptyValues: ContactFormValues = {
  givenName: "",
  surname: "",
  email: "",
  photoUrl: "",
  timeZone: "",
  phoneNumber: "",
  instagramProfileUrl: "",
  tikTokProfileUrl: "",
  facebookProfileUrl: "",
  snapchatProfileUrl: "",
};

function normalizeOptional(value?: string) {
  return value?.trim() || undefined;
}

export function ContactFormPage({ mode }: { mode: "create" | "edit" }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const tenantId = useDirectTenantId();
  const contactQuery = useContact(mode === "edit" ? id : undefined);
  const createContact = useCreateContact();
  const updateContact = useUpdateContact(id ?? "");
  const form = useForm<ContactFormValues>({
    resolver: zodResolver(contactFormSchema),
    defaultValues: emptyValues,
  });

  useEffect(() => {
    if (!contactQuery.data) {
      return;
    }

    form.reset({
      givenName: contactQuery.data.givenName,
      surname: contactQuery.data.surname ?? "",
      email: contactQuery.data.email ?? "",
      photoUrl: contactQuery.data.photoUrl ?? "",
      timeZone: contactQuery.data.timeZone ?? "",
      phoneNumber: contactQuery.data.phoneNumber ?? "",
      instagramProfileUrl: contactQuery.data.instagramProfileUrl ?? "",
      tikTokProfileUrl: contactQuery.data.tikTokProfileUrl ?? "",
      facebookProfileUrl: contactQuery.data.facebookProfileUrl ?? "",
      snapchatProfileUrl: contactQuery.data.snapchatProfileUrl ?? "",
    });
  }, [contactQuery.data, form]);

  const backTo = "/app/direct/contacts";
  const data = contactQuery.data;

  return (
    <DetailLayout
      header={
        <PageHeader
          title={
            mode === "create"
              ? "New contact"
              : data
                ? `${translateText("Edit")} ${data.givenName}`
                : "Edit contact"
          }
          description="Keep one reliable customer identity for every Direct conversation."
          descriptionMode="hint"
          backTo={backTo}
        />
      }
      sidebar={
        data ? (
          <Card>
            <CardHeader>
              <CardHeading>
                <CardTitle>Contact summary</CardTitle>
              </CardHeading>
            </CardHeader>
            <CardContent>
              <KeyValueList
                items={[
                  { label: "Contact ID", value: data.id },
                  {
                    label: "Conversations",
                    value: String(data.conversationCount),
                  },
                  {
                    label: "Primary contact method",
                    value: data.email ?? data.phoneNumber ?? "Not provided",
                  },
                ]}
              />
            </CardContent>
          </Card>
        ) : undefined
      }
    >
      {!tenantId ? (
        <ModuleUnavailableState module="Direct" />
      ) : contactQuery.isError ? (
        <ErrorState
          title="Unable to load contact"
          error={contactQuery.error}
          retry={() => void contactQuery.refetch()}
        />
      ) : mode === "edit" && contactQuery.isLoading ? (
        <FormCardSkeleton fields={10} />
      ) : (
        <Card>
          <CardHeader>
            <CardHeading>
              <CardTitle>Contact profile</CardTitle>
            </CardHeading>
          </CardHeader>
          <CardContent>
            <Form {...form}>
              <form
                className="space-y-6"
                onSubmit={form.handleSubmit(async (values) => {
                  const body = {
                    givenName: values.givenName.trim(),
                    surname: normalizeOptional(values.surname),
                    email: normalizeOptional(values.email),
                    photoUrl: normalizeOptional(values.photoUrl),
                    timeZone: normalizeOptional(values.timeZone),
                    phoneNumber: normalizeOptional(values.phoneNumber),
                    instagramProfileUrl: normalizeOptional(
                      values.instagramProfileUrl,
                    ),
                    tikTokProfileUrl: normalizeOptional(
                      values.tikTokProfileUrl,
                    ),
                    facebookProfileUrl: normalizeOptional(
                      values.facebookProfileUrl,
                    ),
                    snapchatProfileUrl: normalizeOptional(
                      values.snapchatProfileUrl,
                    ),
                  };

                  if (mode === "create") {
                    await createContact.mutateAsync(body);
                  } else {
                    await updateContact.mutateAsync(body);
                  }
                  navigate(backTo);
                })}
              >
                <FormSectionHeading
                  title="Identity"
                  description="Store the stable name and contact details used to recognize this person."
                />
                <div className="grid gap-5 md:grid-cols-2">
                  <TextField
                    control={form.control}
                    name="givenName"
                    label="Given name"
                    description="Required first or preferred name shown throughout Direct."
                  />
                  <TextField
                    control={form.control}
                    name="surname"
                    label="Surname"
                    description="Optional family name used to distinguish contacts with similar given names."
                  />
                  <TextField
                    control={form.control}
                    name="email"
                    type="email"
                    label="Email"
                    description="Optional email address used to identify or reach the contact."
                  />
                  <TextField
                    control={form.control}
                    name="phoneNumber"
                    type="tel"
                    label="Phone number"
                    description="Optional provider-formatted number, including country code when available."
                  />
                  <TextField
                    control={form.control}
                    name="timeZone"
                    label="Time zone"
                    description="Optional IANA time zone such as America/Vancouver for scheduling context."
                  />
                  <TextField
                    control={form.control}
                    name="photoUrl"
                    type="url"
                    label="Photo URL"
                    description="Optional absolute URL for the contact avatar supplied by a trusted provider."
                  />
                </div>

                <FormSectionHeading
                  title="Social profiles"
                  description="Link known public profiles without duplicating provider credentials."
                />
                <div className="grid gap-5 md:grid-cols-2">
                  <TextField
                    control={form.control}
                    name="instagramProfileUrl"
                    type="url"
                    label="Instagram profile URL"
                    description="Optional canonical Instagram profile for identity context."
                  />
                  <TextField
                    control={form.control}
                    name="tikTokProfileUrl"
                    type="url"
                    label="TikTok profile URL"
                    description="Optional canonical TikTok profile for identity context."
                  />
                  <TextField
                    control={form.control}
                    name="facebookProfileUrl"
                    type="url"
                    label="Facebook profile URL"
                    description="Optional canonical Facebook profile for identity context."
                  />
                  <TextField
                    control={form.control}
                    name="snapchatProfileUrl"
                    type="url"
                    label="Snapchat profile URL"
                    description="Optional canonical Snapchat profile for identity context."
                  />
                </div>

                <div className="flex flex-wrap gap-3">
                  <Button
                    type="submit"
                    disabled={
                      createContact.isPending || updateContact.isPending
                    }
                  >
                    <Save className="size-4" />
                    {translateText(
                      mode === "create" ? "Create contact" : "Save changes",
                    )}
                  </Button>
                  <Button asChild variant="outline">
                    <Link to={backTo}>
                      <X className="size-4" />
                      {translateText("Cancel")}
                    </Link>
                  </Button>
                </div>
              </form>
            </Form>
          </CardContent>
        </Card>
      )}
    </DetailLayout>
  );
}
