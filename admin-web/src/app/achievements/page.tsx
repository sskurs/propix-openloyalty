'use client';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";

export default function AchievementsPage() {
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Achievements & Gamification</h1>
      <Card>
        <CardHeader>
          <CardTitle>Coming Soon</CardTitle>
          <CardDescription>This page will be used to configure badges, missions, and challenges to drive member engagement.</CardDescription>
        </CardHeader>
        <CardContent>
          <p>The functionality for this page will be implemented in a future step. Stay tuned!</p>
        </CardContent>
      </Card>
    </div>
  );
}
