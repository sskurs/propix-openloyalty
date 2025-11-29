'use client';

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Wallet, WalletTransaction, ExpiringPoint } from "@/types";

// Mock Data based on your specification
const mockWalletData: {
  points: Wallet;
  cashback: Wallet;
  transactions: WalletTransaction[];
  expiringPoints: ExpiringPoint[];
} = {
  points: {
    id: 'points-wallet',
    type: 'POINTS',
    balance: 6500,
    expiringSoon: 500,
    nextExpiryDate: '2025-03-31',
  },
  cashback: {
    id: 'cashback-wallet',
    type: 'CASHBACK',
    balance: 120.00,
    currency: 'INR',
  },
  transactions: [
    {
      id: 'txn-1',
      date: "2025-02-05T10:32:00Z",
      walletType: "POINTS",
      direction: "EARN",
      amount: 200,
      balanceAfter: 6500,
      reason: "Purchase",
      referenceType: "ORDER",
      referenceId: "ORD-1234",
      campaignId: 42
    },
    {
      id: 'txn-2',
      date: "2025-02-04T16:09:00Z",
      walletType: "POINTS",
      direction: "SPEND",
      amount: 100,
      balanceAfter: 6300,
      reason: "Reward redemption",
      referenceType: "REWARD",
      referenceId: "REW-101"
    },
    {
        id: 'txn-3',
        date: "2025-02-03T11:00:00Z",
        walletType: "CASHBACK",
        direction: "EARN",
        amount: 20,
        balanceAfter: 120.00,
        reason: "Bonus cashback",
        referenceType: "CAMPAIGN",
        referenceId: "CAMP-2025"
    },
    {
        id: 'txn-4',
        date: "2025-02-01T09:00:00Z",
        walletType: "CASHBACK",
        direction: "SPEND",
        amount: 50,
        balanceAfter: 100.00,
        reason: "Purchase offset",
        referenceType: "ORDER",
        referenceId: "ORD-1230"
    }
  ],
  expiringPoints: [
    {
      expiryDate: "2025-03-31",
      points: 200,
      source: "Campaign: Diwali Booster"
    },
    {
      expiryDate: "2025-04-15",
      points: 300,
      source: "Base earn"
    }
  ]
};

function StatCard({ title, value, subtext }: { title: string, value: string | number, subtext: string }) {
    return (
        <Card>
            <CardHeader className="pb-2">
                <p className="text-sm text-muted-foreground">{title}</p>
            </CardHeader>
            <CardContent>
                <p className="text-3xl font-bold">{value}</p>
                <p className="text-xs text-muted-foreground">{subtext}</p>
            </CardContent>
        </Card>
    );
}

export default function WalletsPage() {
  return (
    <div className="space-y-8">
      {/* Wallet Overview */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
        <StatCard title="Points" value={mockWalletData.points.balance.toLocaleString()} subtext="Total points available" />
        <StatCard title="Cashback" value={`${mockWalletData.cashback.balance.toFixed(2)} ${mockWalletData.cashback.currency}`} subtext="Available cashback" />
        <StatCard title="Expiring Points" value={`${mockWalletData.points.expiringSoon} pts`} subtext={`Expiring in next 30 days`} />
      </div>

      {/* Expiring Points Section */}
      <Card>
        <CardHeader>
            <CardTitle>Expiring Points</CardTitle>
        </CardHeader>
        <CardContent>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Expiry Date</TableHead>
                        <TableHead>Points</TableHead>
                        <TableHead>Source</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {mockWalletData.expiringPoints.map((p, i) => (
                        <TableRow key={i}>
                            <TableCell>{p.expiryDate}</TableCell>
                            <TableCell>{p.points}</TableCell>
                            <TableCell>{p.source}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </CardContent>
      </Card>

      {/* Wallet Ledger */}
      <Card>
        <CardHeader>
            <CardTitle>Wallet Ledger</CardTitle>
        </CardHeader>
        <CardContent>
            <Table>
                <TableHeader>
                    <TableRow>
                        <TableHead>Date/Time</TableHead>
                        <TableHead>Wallet Type</TableHead>
                        <TableHead>Direction</TableHead>
                        <TableHead>Amount</TableHead>
                        <TableHead>Balance After</TableHead>
                        <TableHead>Reason</TableHead>
                        <TableHead>Reference</TableHead>
                    </TableRow>
                </TableHeader>
                <TableBody>
                    {mockWalletData.transactions.map(tx => (
                        <TableRow key={tx.id}>
                            <TableCell>{new Date(tx.date).toLocaleString()}</TableCell>
                            <TableCell><Badge variant="secondary">{tx.walletType}</Badge></TableCell>
                            <TableCell><Badge variant={tx.direction === 'EARN' ? 'default' : 'destructive'}>{tx.direction}</Badge></TableCell>
                            <TableCell>{tx.direction === 'EARN' ? '+' : '-'}{tx.amount}</TableCell>
                            <TableCell>{tx.balanceAfter}</TableCell>
                            <TableCell>{tx.reason}</TableCell>
                            <TableCell>{tx.referenceType}: {tx.referenceId}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </CardContent>
      </Card>

    </div>
  );
}
