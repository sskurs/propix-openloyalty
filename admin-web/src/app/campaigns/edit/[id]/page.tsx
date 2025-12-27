'use client';

import { useParams } from 'next/navigation';
import { useEffect, useState } from 'react';
import { CampaignWizard } from '@/components/campaigns/campaign-wizard';
import { IconLoader2 } from '@tabler/icons-react';
import { toast } from 'sonner';

export default function EditCampaignPage() {
    const { id } = useParams();
    const [campaign, setCampaign] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCampaign = async () => {
            try {
                const res = await fetch(`/api/campaigns/${id}`);
                if (res.ok) {
                    const data = await res.json();
                    setCampaign(data);
                } else {
                    toast.error('Failed to fetch campaign details');
                }
            } catch (error) {
                toast.error('An error occurred while fetching campaign');
            } finally {
                setLoading(false);
            }
        };

        if (id) fetchCampaign();
    }, [id]);

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <IconLoader2 className="animate-spin h-8 w-8 text-muted-foreground" />
            </div>
        );
    }

    if (!campaign) {
        return <div className="text-center py-12">Campaign not found.</div>;
    }

    return (
        <div className="container mx-auto py-6">
            <h1 className="text-2xl font-bold mb-6">Edit Campaign</h1>
            <CampaignWizard initialData={campaign} isEdit={true} />
        </div>
    );
}
