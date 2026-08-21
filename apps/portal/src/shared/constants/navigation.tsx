import {
  BookOpenCheck,
  CircleDollarSign,
  ContactRound,
  FolderKanban,
  Home,
  MessagesSquare,
  PlugZap,
  RadioTower,
  Settings,
  Tags,
  Users,
  UserRound,
  Waypoints,
  type LucideIcon,
} from "lucide-react";

export type NavigationItem = {
  key: string;
  label: string;
  description: string;
  path: string;
  icon: LucideIcon;
  activePaths?: string[];
  children?: NavigationItem[];
};

export type NavigationGroup = {
  key: string;
  label: string;
  items: NavigationItem[];
};

export const portalNavigationGroups: NavigationGroup[] = [
  {
    key: "modules",
    label: "Modules",
    items: [
      {
        key: "qna",
        label: "Base",
        description: "Primary knowledge base",
        path: "/app/dashboard",
        icon: BookOpenCheck,
        children: [
          {
            key: "dashboard",
            label: "Home",
            description: "Attention queue and value proof",
            path: "/app/dashboard",
            icon: Home,
          },
          {
            key: "spaces",
            label: "Spaces",
            description: "Knowledge boundaries",
            path: "/app/spaces",
            icon: FolderKanban,
            activePaths: [
              "/app/spaces",
              "/app/questions",
              "/app/answers",
              "/app/activity",
            ],
          },
          {
            key: "sources",
            label: "Sources",
            description: "Evidence catalog",
            path: "/app/sources",
            icon: Waypoints,
          },
          {
            key: "tags",
            label: "Tags",
            description: "Reusable taxonomy",
            path: "/app/tags",
            icon: Tags,
          },
          {
            key: "mcp",
            label: "MCP",
            description: "Agent tools and local clients",
            path: "/app/mcp",
            icon: PlugZap,
          },
        ],
      },
      {
        key: "direct",
        label: "Direct",
        description: "Private customer conversations",
        path: "/app/direct/conversations",
        icon: MessagesSquare,
        children: [
          {
            key: "conversations",
            label: "Conversations",
            description: "Private support timelines",
            path: "/app/direct/conversations",
            icon: MessagesSquare,
          },
          {
            key: "contacts",
            label: "Contacts",
            description: "Customer identities",
            path: "/app/direct/contacts",
            icon: ContactRound,
          },
        ],
      },
      {
        key: "broadcast",
        label: "Broadcast",
        description: "Public and community interactions",
        path: "/app/broadcast/threads",
        icon: RadioTower,
        children: [
          {
            key: "threads",
            label: "Threads",
            description: "Public interaction timelines",
            path: "/app/broadcast/threads",
            icon: RadioTower,
          },
        ],
      },
    ],
  },
  {
    key: "administration",
    label: "Administration",
    items: [
      {
        key: "members",
        label: "Members",
        description: "People and workspace roles",
        path: "/app/members",
        icon: Users,
      },
      {
        key: "billing",
        label: "Billing",
        description: "Plan, invoices, and payments",
        path: "/app/billing",
        icon: CircleDollarSign,
      },
      {
        key: "settings",
        label: "Settings",
        description: "Workspace and integrations",
        path: "/app/settings/tenant",
        icon: Settings,
        activePaths: [
          "/app/settings/tenant",
          "/app/settings/channel-connections",
          "/app/settings/general",
          "/app/settings/security",
        ],
      },
    ],
  },
  {
    key: "account",
    label: "Account",
    items: [
      {
        key: "profile",
        label: "Profile",
        description: "Language, time zone, and contact info",
        path: "/app/settings/profile",
        icon: UserRound,
        activePaths: ["/app/settings/profile"],
      },
    ],
  },
];

export const portalNavigation = portalNavigationGroups.flatMap(
  (group) => group.items,
);

export function findPortalNavigationPath(
  key: string,
  items: NavigationItem[] = portalNavigation,
): NavigationItem[] {
  for (const item of items) {
    if (item.key === key) {
      return [item];
    }

    if (item.children) {
      const childPath = findPortalNavigationPath(key, item.children);

      if (childPath.length > 0) {
        return [item, ...childPath];
      }
    }
  }

  return [];
}
