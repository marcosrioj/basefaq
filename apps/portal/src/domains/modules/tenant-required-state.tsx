import { EmptyState } from "@/shared/ui/placeholder-state";

export function TenantRequiredState() {
  return (
    <EmptyState
      title="No workspace selected"
      description="Select a workspace to continue."
    />
  );
}
