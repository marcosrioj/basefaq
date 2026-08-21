import type { RouteObject } from "react-router-dom";
import { ChannelConnectionFormPage } from "@/domains/channel-connections/channel-connection-form-page";
import { ChannelConnectionListPage } from "@/domains/channel-connections/channel-connection-list-page";

export const ChannelConnectionRoutes: RouteObject[] = [
  {
    path: "channel-connections",
    element: <ChannelConnectionListPage />,
    handle: {
      title: "Channel connections",
      breadcrumb: "Channel connections",
      navKey: "settings",
    },
  },
  {
    path: "channel-connections/new",
    element: <ChannelConnectionFormPage mode="create" />,
    handle: {
      title: "New channel connection",
      breadcrumb: "Create",
      navKey: "settings",
    },
  },
  {
    path: "channel-connections/:id/edit",
    element: <ChannelConnectionFormPage mode="edit" />,
    handle: {
      title: "Edit channel connection",
      breadcrumb: "Edit",
      navKey: "settings",
    },
  },
];
