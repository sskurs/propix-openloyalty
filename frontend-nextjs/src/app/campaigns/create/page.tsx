import { CampaignWizard } from '@/components/campaigns/campaign-wizard';

export default function CreateCampaignPage() {
  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Create New Campaign</h1>
      <CampaignWizard />
    </div>
  );
}
