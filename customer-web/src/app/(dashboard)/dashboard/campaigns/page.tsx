"use client";

import { useEffect, useState } from "react";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Calendar, ArrowRight, Zap, Loader2 } from "lucide-react"

interface Campaign {
    id: string;
    name: string;
    description: string;
    type: string;
    validTo: string;
}

export default function CampaignsPage() {
    const [campaigns, setCampaigns] = useState<Campaign[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch("/api/customer/campaigns")
            .then(res => res.json())
            .then(data => {
                setCampaigns(data);
                setLoading(false);
            })
            .catch(err => {
                console.error("Error fetching campaigns:", err);
                setLoading(false);
            });
    }, []);

    if (loading) {
        return (
            <div className="flex items-center justify-center h-64">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <h1 className="text-3xl font-bold tracking-tight">Active Campaigns</h1>
            </div>

            {campaigns.length === 0 ? (
                <div className="text-center py-12 border-2 border-dashed rounded-lg bg-muted/50">
                    <p className="text-muted-foreground">No active campaigns at the moment. Check back later!</p>
                </div>
            ) : (
                <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
                    {campaigns.map((campaign) => (
                        <Card key={campaign.id} className="flex flex-col border-primary/10 hover:border-primary/30 transition-all group">
                            <CardHeader className="pb-4">
                                <div className="flex justify-between items-start mb-2">
                                    <Badge variant="outline" className="bg-primary/5 text-primary border-primary/20 capitalize">
                                        {campaign.type?.replace('-', ' ') || 'Campaign'}
                                    </Badge>
                                    <Zap className="h-5 w-5 text-amber-500 fill-amber-500" />
                                </div>
                                <CardTitle className="group-hover:text-primary transition-colors">{campaign.name}</CardTitle>
                                <CardDescription className="line-clamp-2 mt-2">{campaign.description || 'Enjoy exclusive rewards with this campaign.'}</CardDescription>
                            </CardHeader>
                            <CardContent className="flex-1">
                                <div className="flex items-center gap-2 text-sm text-muted-foreground">
                                    <Calendar className="h-4 w-4" />
                                    <span>Ends: {new Date(campaign.validTo).toLocaleDateString()}</span>
                                </div>
                            </CardContent>
                            <CardFooter className="pt-4 border-t border-muted">
                                <Button variant="ghost" className="w-full group/btn">
                                    View Details <ArrowRight className="h-4 w-4 ml-2 transition-transform group-hover/btn:translate-x-1" />
                                </Button>
                            </CardFooter>
                        </Card>
                    ))}
                </div>
            )}
        </div>
    )
}
