import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Wallet, ArrowUpRight, ArrowDownRight, TrendingUp } from "lucide-react"

const pointsHistory = [
    { id: 1, type: 'earn', description: 'Purchase at Store #123', points: 150, date: '2023-10-25' },
    { id: 2, type: 'redeem', description: 'Free Coffee Reward', points: -50, date: '2023-10-20' },
    { id: 3, type: 'earn', description: 'Referral Bonus', points: 500, date: '2023-10-15' },
    { id: 4, type: 'earn', description: 'Birthday Bonus', points: 1000, date: '2023-10-01' },
]

export default function PointsPage() {
    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Points Balance</h1>
            </div>

            <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
                <Card className="bg-primary/5 border-primary/20">
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium text-primary">Current Balance</CardTitle>
                        <Wallet className="h-4 w-4 text-primary" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold text-primary">2,450</div>
                        <p className="text-xs text-muted-foreground mt-1">+15% from last month</p>
                    </CardContent>
                </Card>
                <Card>
                    <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                        <CardTitle className="text-sm font-medium">Lifetime Earned</CardTitle>
                        <TrendingUp className="h-4 w-4 text-muted-foreground" />
                    </CardHeader>
                    <CardContent>
                        <div className="text-2xl font-bold text-foreground">12,500</div>
                    </CardContent>
                </Card>
            </div>

            <Card>
                <CardHeader>
                    <CardTitle>Points History</CardTitle>
                    <CardDescription>A detailed log of your points transactions.</CardDescription>
                </CardHeader>
                <CardContent>
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Date</TableHead>
                                <TableHead>Description</TableHead>
                                <TableHead>Type</TableHead>
                                <TableHead className="text-right">Points</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            {pointsHistory.map((item) => (
                                <TableRow key={item.id}>
                                    <TableCell className="font-medium">{item.date}</TableCell>
                                    <TableCell>{item.description}</TableCell>
                                    <TableCell>
                                        <Badge variant={item.type === 'earn' ? 'success' : 'destructive'} className="capitalize">
                                            {item.type}
                                        </Badge>
                                    </TableCell>
                                    <TableCell className={`text-right font-bold ${item.points > 0 ? 'text-green-600' : 'text-red-600'}`}>
                                        {item.points > 0 ? '+' : ''}{item.points}
                                    </TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                </CardContent>
            </Card>
        </div>
    )
}
