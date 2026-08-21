import { EmptyState } from "@/shared/ui/placeholder-state";

export function ModuleUnavailableState({
  module,
}: {
  module: "Direct" | "Broadcast";
}) {
  const isDirect = module === "Direct";

  return (
    <EmptyState
      title={
        isDirect ? "Direct is not available" : "Broadcast is not available"
      }
      description={
        isDirect
          ? "This workspace does not have an active Direct tenant. Ask a workspace administrator to enable the module."
          : "This workspace does not have an active Broadcast tenant. Ask a workspace administrator to enable the module."
      }
    />
  );
}
