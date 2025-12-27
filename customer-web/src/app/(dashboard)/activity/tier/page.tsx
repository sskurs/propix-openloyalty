import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { TrendingUp, Award, ArrowUp, Milestone } from "lucide-react"

const tierChanges = [
    { id: 1, action: 'Promoted', oldTier: 'Silver', newTier: 'Gold', reason: 'Reached 2500 Lifetime Points', date: '2023-10-15' },
    { id: 2, action: 'Promoted', oldTier: 'Bronze', newTier: 'Silver', reason: 'Initial Activity Milestone', date: '2023-09-01' },
    { id: 3, action: 'Joined', oldTier: '-', newTier: 'Bronze', reason: 'Account Registration', date: '2023-08-20' },
]

export default function TierChangesPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Tier Changes</h1>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Your Tier Progress</CardTitle>
                    <CardDescription>Track your journey through our loyalty tiers.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Action</TableHead>
                                <TableHead>From</TableHead>
                                <TableHead>To</TableHead>
                                <TableHead>Reason</TableHead>
                                <TableHead className="text-right">Date</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {tierChanges.map((change) => (
                                <TableRow key={change.id} className="group hover:bg-muted/50 transition-colors">
                                    <TableCell>
                                        <div className="flex items-center gap-2">
                                            {change.action === 'Promoted' && <ArrowUp className="h-4 w-4 text-green-500 animate-bounce" />}
                                            {change.action === 'Joined' && <Milestone className="h-4 w-4 text-blue-500" />}
                                            <span className="font-extrabold uppercase tracking-tighter text-xs">{change.action}</span>
                                        </div>
                                    </TableCell>
                                    <TableCell className="text-muted-foreground">{change.oldTier}</TableCell>
                                    <TableCell>
                                        <Badge className="bg-gradient-to-r from-primary to-primary/80 font-black px-3">
                                            {change.newTier}
                                        </Badge>
                                    </TableCell>
                                    <TableCell className="text-xs italic text-muted-foreground">{change.reason}</TableCell>
                                    <TableCell className="text-right font-medium">{change.date}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>

            <div className="flex justify-center mt-10">
                <div className="flex items-center gap-4 p-6 border-2 border-dashed rounded-3xl bg-muted/20 opacity-50">
                    <Award className="h-10 w-10 text-muted-foreground" />
                    <div>
                        <h3 className="font-bold text-muted-foreground">Keep going!</h3>
                        <p className="text-xs text-muted-foreground">Platinum Tier is just around the corner.</p>
                    </div>
                </div>
            </div>
        </div>
    )
}
