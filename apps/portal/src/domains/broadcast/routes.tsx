import type { RouteObject } from "react-router-dom";
import { BroadcastThreadDetailPage } from "@/domains/broadcast/thread-detail-page";
import { BroadcastThreadFormPage } from "@/domains/broadcast/thread-form-page";
import { BroadcastThreadListPage } from "@/domains/broadcast/thread-list-page";

export const BroadcastRoutes: RouteObject[] = [
  {
    path: "broadcast/threads",
    element: <BroadcastThreadListPage />,
    handle: {
      title: "Broadcast threads",
      breadcrumb: "Broadcast threads",
      navKey: "broadcast",
    },
  },
  {
    path: "broadcast/threads/new",
    element: <BroadcastThreadFormPage mode="create" />,
    handle: {
      title: "New Broadcast thread",
      breadcrumb: "Create",
      navKey: "broadcast",
    },
  },
  {
    path: "broadcast/threads/:id",
    element: <BroadcastThreadDetailPage />,
    handle: {
      title: "Broadcast thread",
      breadcrumb: "Broadcast thread",
      navKey: "broadcast",
    },
  },
  {
    path: "broadcast/threads/:id/edit",
    element: <BroadcastThreadFormPage mode="edit" />,
    handle: {
      title: "Edit Broadcast thread",
      breadcrumb: "Edit",
      navKey: "broadcast",
    },
  },
];
