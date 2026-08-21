import type { RouteObject } from "react-router-dom";
import { ContactFormPage } from "@/domains/direct/contact-form-page";
import { ContactListPage } from "@/domains/direct/contact-list-page";
import { ConversationDetailPage } from "@/domains/direct/conversation-detail-page";
import { ConversationFormPage } from "@/domains/direct/conversation-form-page";
import { ConversationListPage } from "@/domains/direct/conversation-list-page";

export const DirectRoutes: RouteObject[] = [
  {
    path: "direct/conversations",
    element: <ConversationListPage />,
    handle: {
      title: "Conversations",
      breadcrumb: "Conversations",
      navKey: "direct",
    },
  },
  {
    path: "direct/conversations/new",
    element: <ConversationFormPage mode="create" />,
    handle: {
      title: "New conversation",
      breadcrumb: "Create",
      navKey: "direct",
    },
  },
  {
    path: "direct/conversations/:id",
    element: <ConversationDetailPage />,
    handle: {
      title: "Conversation",
      breadcrumb: "Conversation",
      navKey: "direct",
    },
  },
  {
    path: "direct/conversations/:id/edit",
    element: <ConversationFormPage mode="edit" />,
    handle: {
      title: "Edit conversation",
      breadcrumb: "Edit",
      navKey: "direct",
    },
  },
  {
    path: "direct/contacts",
    element: <ContactListPage />,
    handle: {
      title: "Contacts",
      breadcrumb: "Contacts",
      navKey: "direct",
    },
  },
  {
    path: "direct/contacts/new",
    element: <ContactFormPage mode="create" />,
    handle: {
      title: "New contact",
      breadcrumb: "Create",
      navKey: "direct",
    },
  },
  {
    path: "direct/contacts/:id/edit",
    element: <ContactFormPage mode="edit" />,
    handle: {
      title: "Edit contact",
      breadcrumb: "Edit",
      navKey: "direct",
    },
  },
];
