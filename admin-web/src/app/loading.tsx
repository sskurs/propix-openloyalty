import { Loader } from "lucide-react";

export default function Loading() {
  // You can add any UI inside Loading, including a Skeleton.
  return (
    <div className="flex h-full w-full items-center justify-center">
      <Loader className="h-12 w-12 animate-spin text-primary" />
    </div>
  );
}
