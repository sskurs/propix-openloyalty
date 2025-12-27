"use client"

import { useAuth } from "@/hooks/use-auth"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

export default function WalletPage() {
    const { member } = useAuth()

    return (
        <div className="space-y-6">
            <h2 className="text-3xl font-bold tracking-tight">My Wallet</h2>
            <div className="grid gap-4 md:grid-cols-2">
                {member?.wallets?.map((wallet) => (
                    <Card key={wallet.id}>
                        <CardHeader>
                            <CardTitle className="capitalize">{wallet.type} Wallet</CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="text-3xl font-bold">
                                {wallet.type === 'cashback' ? `$${wallet.balance}` : `${wallet.balance} pts`}
                            </div>
                        </CardContent>
                    </Card>
                ))}
            </div>
        </div>
    )
}
