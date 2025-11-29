import { Card, CardContent, CardHeader, CardTitle, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';

export default function CreateRewardPage() {
  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Create New Reward</h1>
      <Card className="max-w-2xl mx-auto">
        <CardHeader>
          <CardTitle>Reward Definition</CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="name">Reward Name</Label>
            <Input id="name" placeholder="e.g., Free Coffee" required />
          </div>
          <div className="space-y-2">
            <Label htmlFor="type">Type</Label>
            <Select required>
              <SelectTrigger id="type">
                <SelectValue placeholder="Select reward type" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="coupon">Coupon</SelectItem>
                <SelectItem value="partner">Partner</SelectItem>
                <SelectItem value="physical">Physical Product</SelectItem>
              </SelectContent>
            </Select>
          </div>
          <div className="space-y-2">
            <Label htmlFor="points">Points Cost</Label>
            <Input id="points" type="number" placeholder="e.g., 100" required />
          </div>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="validity-days">Validity (Days)</Label>
              <Input id="validity-days" type="number" placeholder="e.g., 30" />
            </div>
            <div className="space-y-2">
                <Label htmlFor="validity-range">Or Date Range</Label>
                <div className="flex space-x-2">
                    <Input id="validity-start" type="date" />
                    <Input id="validity-end" type="date" />
                </div>
            </div>
          </div>
          <div className="space-y-4">
            <Label>Visibility Rules (Optional)</Label>
            <div className="grid grid-cols-2 gap-4">
                <div className="space-y-2">
                    <Label className="text-sm">Tiers</Label>
                    <div className="flex items-center space-x-2"><Checkbox id="tier-gold" /><Label htmlFor="tier-gold">Gold</Label></div>
                    <div className="flex items-center space-x-2"><Checkbox id="tier-silver" /><Label htmlFor="tier-silver">Silver</Label></div>
                </div>
                <div className="space-y-2">
                    <Label className="text-sm">Tags</Label>
                    <div className="flex items-center space-x-2"><Checkbox id="tag-vip" /><Label htmlFor="tag-vip">VIP</Label></div>
                </div>
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex justify-end gap-2">
            <Button variant="secondary">Save Draft</Button>
            <Button>Activate</Button>
        </CardFooter>
      </Card>
    </div>
  );
}
