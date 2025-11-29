import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";

// Helper component for KPI cards
function KpiCard({ title, value, change }: { title: string; value: string; change?: string }) {
  return (
    <Card>
      <CardHeader className="pb-2">
        <p className="text-sm text-muted-foreground">{title}</p>
      </CardHeader>
      <CardContent>
        <p className="text-3xl font-bold">{value}</p>
        {change && <p className="text-xs text-muted-foreground">{change}</p>}
      </CardContent>
    </Card>
  );
}

// Mock data for recent activity
const recentActivities = [
  { type: "New Member", description: "John Doe (john.d@example.com) just registered.", time: "5m ago" },
  { type: "Redemption", description: "Jane Smith redeemed 5,000 points for a $50 Voucher.", time: "1h ago" },
  { type: "Tier Upgrade", description: "Alex Johnson has reached the Gold Tier.", time: "3h ago" },
  { type: "Suspicious", description: "High-frequency earning activity detected for member ID 12345.", time: "8h ago", variant: "destructive" as const },
  { type: "New Member", description: "Emily White has registered.", time: "1d ago" },
];

export default function DashboardPage() {
  return (
    <div className="space-y-8">
      {/* KPI Section */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-6 gap-4">
        <KpiCard title="Total Members" value="12,458" change="+215 this month" />
        <KpiCard title="Active Members" value="9,840" change="79% of total" />
        <KpiCard title="Newly Joined (30d)" value="480" />
        <KpiCard title="Total Points Issued" value="1.2M" />
        <KpiCard title="Total Points Redeemed" value="850K" />
        <KpiCard title="Points Breakage (30d)" value="50,210" />
      </div>

      {/* Charts and Recent Activity Section */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2 space-y-8">
            {/* Chart Placeholders */}
            <Card>
                <CardHeader><CardTitle>Points Issuance vs. Redemption (Weekly)</CardTitle></CardHeader>
                <CardContent>
                    <p className="text-muted-foreground">[Chart component will be implemented here]</p>
                </CardContent>
            </Card>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                <Card>
                    <CardHeader><CardTitle>Campaign Performance</CardTitle></CardHeader>
                    <CardContent>
                        <p className="text-muted-foreground">[Chart component will be implemented here]</p>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader><CardTitle>Tier Distribution</CardTitle></CardHeader>
                    <CardContent>
                        <p className="text-muted-foreground">[Chart component will be implemented here]</p>
                    </CardContent>
                </Card>
            </div>
        </div>

        {/* Recent Activity Feed */}
        <div className="lg:col-span-1">
            <Card>
                <CardHeader><CardTitle>Recent Activity</CardTitle></CardHeader>
                <CardContent>
                    <Table>
                        <TableBody>
                            {recentActivities.map((activity, index) => (
                                <TableRow key={index}>
                                    <TableCell>
                                        <div className="font-medium">{activity.description}</div>
                                        <div className="text-sm text-muted-foreground flex items-center gap-2 mt-1">
                                            <Badge variant={activity.variant || 'secondary'}>{activity.type}</Badge>
                                            <span>{activity.time}</span>
                                        </div>
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
      </div>
    </div>
  );
}
