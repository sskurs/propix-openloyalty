import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

export default function SettingsPage() {
  return (
    <Card>
      <CardHeader>
        <CardTitle>System Settings</CardTitle>
        <p className="text-sm text-muted-foreground">This page will be used to configure system-wide settings.</p>
      </CardHeader>
      <CardContent>
        <p>Functionality for this page will be implemented in a future step.</p>
      </CardContent>
    </Card>
  );
}
