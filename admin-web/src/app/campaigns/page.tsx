'use client';

import * as React from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { ToggleGroup, ToggleGroupItem } from '@/components/ui/toggle-group';
import { IconDotsVertical, IconFilter, IconPlus, IconLoader2 } from '@tabler/icons-react';
import Link from 'next/link';
import { toast } from 'sonner';

interface Campaign {
  id: string;
  code: string;
  name: string;
  priority: number;
  status: string;
  startAt: string;
  endAt?: string | null;
}

export default function CampaignsPage() {
  const [campaigns, setCampaigns] = React.useState<Campaign[]>([]);
  const [filterType, setFilterType] = React.useState<string>('all');
  const [loading, setLoading] = React.useState(true);

  const fetchCampaigns = async () => {
    try {
      const res = await fetch('/api/campaigns');
      if (res.ok) {
        const data = await res.json();
        setCampaigns(data);
      }
    } catch (error) {
      toast.error('Failed to fetch campaigns');
    } finally {
      setLoading(false);
    }
  };

  React.useEffect(() => {
    fetchCampaigns();
  }, []);

  // ... inside CampaignsPage component

  const handleDelete = async (id: string) => {
    if (!confirm('Are you sure you want to delete this campaign?')) return;

    try {
      const res = await fetch(`/api/campaigns/${id}`, { method: 'DELETE' });
      if (res.ok) {
        toast.success('Campaign deleted successfully');
        setCampaigns(campaigns.filter(c => c.id !== id));
      } else {
        toast.error('Failed to delete campaign');
      }
    } catch (error) {
      toast.error('An error occurred during deletion');
    }
  };

  const handleStatusChange = async (campaign: Campaign, newStatus: string) => {
    try {
      const updatedCampaign = { ...campaign, status: newStatus };
      const res = await fetch(`/api/campaigns/${campaign.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updatedCampaign),
      });

      if (res.ok) {
        toast.success(`Campaign status updated to ${newStatus}`);
        setCampaigns(campaigns.map(c => c.id === campaign.id ? updatedCampaign : c));
      } else {
        toast.error('Failed to update campaign status');
      }
    } catch (error) {
      toast.error('An error occurred during status update');
    }
  };

  const handleArchive = async (campaign: Campaign) => {
    if (!confirm('Are you sure you want to archive this campaign?')) return;
    await handleStatusChange(campaign, 'ARCHIVED');
  };

  const getComputedStatus = (c: Campaign) => {
    if (c.status === 'ARCHIVED') return 'ARCHIVED';
    if (c.status === 'PAUSED') return 'PAUSED';
    if (c.status === 'DRAFT') return 'DRAFT';

    const NOW = new Date();
    const start = new Date(c.startAt);
    const end = c.endAt ? new Date(c.endAt) : null;

    if (c.status === 'ACTIVE') {
      if (start > NOW) return 'SCHEDULED';
      if (end && end < NOW) return 'COMPLETED';
    }
    return c.status; // Default ACTIVE
  };

  const formatDate = (dateString?: string | null) => {
    if (!dateString) return 'Ongoing';
    return new Date(dateString).toLocaleDateString();
  };

  const filteredCampaigns = campaigns.filter(c => {
    const status = getComputedStatus(c);
    console.log(`Debug Campaign: ${c.name} (${c.id}) | Raw: ${c.status} | Computed: ${status} | Filter: ${filterType}`);

    if (filterType === 'all') return status !== 'ARCHIVED';
    return status.toLowerCase() === filterType.toLowerCase();
  });

  const getBadgeVariant = (status: string) => {
    switch (status) {
      case 'ACTIVE': return 'success';
      case 'SCHEDULED': return 'outline'; // Blue-ish? Using outline for now or need new variant
      case 'COMPLETED': return 'secondary';
      case 'PAUSED': return 'warning';
      case 'ARCHIVED': return 'secondary';
      case 'DRAFT': return 'secondary';
      default: return 'outline';
    }
  };

  // ... (Rest of format) ...

  return (
    <div>
      <div className="flex justify-between items-center mb-4">
        <h1 className="text-2xl font-bold">Campaigns</h1>
        <div className="flex gap-2">
          <Link href="/campaigns/create">
            <Button>
              <IconPlus className="mr-2 h-4 w-4" />
              New
            </Button>
          </Link>
        </div>
      </div>

      <div className="mb-4">
        <ToggleGroup type="single" className="justify-start flex-wrap h-auto gap-0" value={filterType} onValueChange={(v) => v && setFilterType(v)} variant="outline">
          <ToggleGroupItem value="all">All</ToggleGroupItem>
          <ToggleGroupItem value="draft">Draft</ToggleGroupItem>
          <ToggleGroupItem value="scheduled">Scheduled</ToggleGroupItem>
          <ToggleGroupItem value="active">Active</ToggleGroupItem>
          <ToggleGroupItem value="paused">Paused</ToggleGroupItem>
          <ToggleGroupItem value="completed">Completed</ToggleGroupItem>
          <ToggleGroupItem value="archived">Archived</ToggleGroupItem>
        </ToggleGroup>
      </div>

      <div className="rounded-md border">
        {loading ? (
          <div className="p-8 flex justify-center">
            <IconLoader2 className="animate-spin h-8 w-8 text-muted-foreground" />
          </div>
        ) : (
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Code</TableHead>
                <TableHead>Name</TableHead>
                <TableHead>Priority</TableHead>
                <TableHead>Start Date</TableHead>
                <TableHead>End Date</TableHead>
                <TableHead>Status</TableHead>
                <TableHead className="w-[50px]"></TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filteredCampaigns.length === 0 ? (
                <TableRow>
                  <TableCell colSpan={7} className="text-center py-8 text-muted-foreground">
                    No campaigns found.
                  </TableCell>
                </TableRow>
              ) : (
                filteredCampaigns.map((campaign) => {
                  const computedStatus = getComputedStatus(campaign);
                  return (
                    <TableRow key={campaign.id}>
                      <TableCell className="font-mono text-xs">{campaign.code}</TableCell>
                      <TableCell className="font-medium">{campaign.name}</TableCell>
                      <TableCell>{campaign.priority}</TableCell>
                      <TableCell>
                        {formatDate(campaign.startAt)}
                      </TableCell>
                      <TableCell>
                        {formatDate(campaign.endAt)}
                      </TableCell>
                      <TableCell>
                        <Badge variant={getBadgeVariant(computedStatus) as any}>
                          {computedStatus}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <Button variant="ghost" className="h-8 w-8 p-0">
                              <span className="sr-only">Open menu</span>
                              <IconDotsVertical className="h-4 w-4" />
                            </Button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end">
                            <Link href={`/campaigns/edit/${campaign.id}`}>
                              <DropdownMenuItem className="cursor-pointer">Edit</DropdownMenuItem>
                            </Link>
                            {computedStatus === 'ACTIVE' || computedStatus === 'SCHEDULED' ? (
                              <DropdownMenuItem onClick={() => handleStatusChange(campaign, 'PAUSED')}>
                                Pause
                              </DropdownMenuItem>
                            ) : computedStatus === 'PAUSED' ? (
                              <DropdownMenuItem onClick={() => handleStatusChange(campaign, 'ACTIVE')}>
                                Resume
                              </DropdownMenuItem>
                            ) : null}

                            {computedStatus !== 'ARCHIVED' && (
                              <DropdownMenuItem onClick={() => handleArchive(campaign)} className="text-orange-500 cursor-pointer">
                                Archive
                              </DropdownMenuItem>
                            )}

                            <DropdownMenuItem onClick={() => handleDelete(campaign.id)} className="text-destructive cursor-pointer">
                              Delete
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </TableCell>
                    </TableRow>
                  );
                })
              )}
            </TableBody>
          </Table>
        )}
      </div>
    </div>
  );
}
