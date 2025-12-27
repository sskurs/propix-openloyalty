"use client"

import * as React from "react"
import { Star, Calendar } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"

interface Campaign {
    id: string;
    name: string;
    description: string;
    status: string;
    validTo: string;
}

export default function EarnPage() {
    const [campaigns, setCampaigns] = React.useState<Campaign[]>([])
    const [loading, setLoading] = React.useState(true)

    React.useEffect(() => {
        fetch("/api/customer/campaigns")
            .then((res) => res.json())
            .then((data) => {
                setCampaigns(data)
                setLoading(false)
            })
            .catch(() => setLoading(false))
    }, [])

    if (loading) {
        return <div className="p-10">Loading campaigns...</div>
    }

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h2 className="text-3xl font-bold tracking-tight">Earn Points</h2>
                    <p className="text-muted-foreground">
                        Participate in campaigns to boost your loyalty balance.
                    </p>
                </div>
            </div>
            <div className="grid gap-6">
                {campaigns.length === 0 ? (
                    <p className="text-muted-foreground">No active campaigns at the moment.</p>
                ) : (
                    campaigns.map((campaign) => (
                        <Card key={campaign.id}>
                            <CardHeader>
                                <div className="flex items-center justify-between">
                                    <div className="space-y-1">
                                        <CardTitle>{campaign.name}</CardTitle>
                                        <CardDescription>
                                            {campaign.description}
                                        </CardDescription>
                                    </div>
                                    <Badge>{campaign.status || "Active"}</Badge>
                                </div>
                            </CardHeader>
                            <CardContent>
                                <div className="flex items-center gap-4 text-sm text-muted-foreground">
                                    <div className="flex items-center gap-1">
                                        <Calendar className="h-4 w-4" />
                                        <span>Valid until: {new Date(campaign.validTo).toLocaleDateString()}</span>
                                    </div>
                                </div>
                            </CardContent>
                            <CardFooter>
                                <Button>Join Campaign</Button>
                            </CardFooter>
                        </Card>
                    ))
                )}
            </div>
        </div>
    )
}
