'use client';

import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { Checkbox } from '@/components/ui/checkbox';
import { Separator } from '@/components/ui/separator';
import { useState } from 'react';

export default function CreateCouponPage() {
  const [codeType, setCodeType] = useState('GLOBAL');
  const [discountType, setDiscountType] = useState('PERCENT');
  const [applicability, setApplicability] = useState('ORDER');

  return (
    <div>
      <h1 className="text-3xl font-bold mb-8">Create New Coupon</h1>
      <div className="space-y-8 max-w-4xl mx-auto">
        
        {/* Section: Basic Info */}
        <Card>
          <CardHeader>
            <CardTitle>Basic Information</CardTitle>
            <CardDescription>Set the fundamental details for your coupon.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="name">Coupon Name</Label>
              <Input id="name" placeholder="e.g., New Year 10% Off" required />
            </div>
            <div className="space-y-2">
              <Label htmlFor="description">Description (Optional)</Label>
              <Textarea id="description" placeholder="Internal note on what this coupon is for."/>
            </div>
          </CardContent>
        </Card>

        {/* Section: Code & Type */}
        <Card>
          <CardHeader>
            <CardTitle>Code & Type</CardTitle>
            <CardDescription>Define how the coupon code is generated and used.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
             <div className="space-y-2">
                <Label>Code Type</Label>
                <Select value={codeType} onValueChange={setCodeType}>
                  <SelectTrigger><SelectValue /></SelectTrigger>
                  <SelectContent>
                    <SelectItem value="GLOBAL">Global Code (One code for all members)</SelectItem>
                    <SelectItem value="UNIQUE">Unique Code (One code per member)</SelectItem>
                  </SelectContent>
                </Select>
            </div>
            {codeType === 'GLOBAL' && (
                 <div className="space-y-2">
                    <Label htmlFor="global-code">Global Code</Label>
                    <Input id="global-code" placeholder="e.g., NY2025" required />
                </div>
            )}
            {codeType === 'UNIQUE' && (
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                     <div className="space-y-2">
                        <Label htmlFor="unique-prefix">Prefix (Optional)</Label>
                        <Input id="unique-prefix" placeholder="e.g., NY25-" />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="unique-length">Code Length</Label>
                        <Input id="unique-length" type="number" placeholder="e.g., 8" defaultValue={8} />
                    </div>
                </div>
            )}
          </CardContent>
        </Card>

        {/* Section: Discount Definition */}
        <Card>
          <CardHeader>
            <CardTitle>Discount Definition</CardTitle>
            <CardDescription>Specify the value and applicability of the discount.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2">
                    <Label>Discount Type</Label>
                    <Select value={discountType} onValueChange={setDiscountType}>
                    <SelectTrigger><SelectValue /></SelectTrigger>
                    <SelectContent>
                        <SelectItem value="PERCENT">Percentage</SelectItem>
                        <SelectItem value="AMOUNT">Fixed Amount</SelectItem>
                        <SelectItem value="FREE_SHIPPING">Free Shipping</SelectItem>
                    </SelectContent>
                    </Select>
                </div>
                {discountType !== 'FREE_SHIPPING' && <div className="space-y-2">
                    <Label>Discount Value</Label>
                    <Input id="discount-value" type="number" placeholder={discountType === 'PERCENT' ? "10" : "250"} required />
                </div>}
            </div>
            <div className="space-y-2">
                <Label>Applicability</Label>
                <Select value={applicability} onValueChange={setApplicability}>
                <SelectTrigger><SelectValue /></SelectTrigger>
                <SelectContent>
                    <SelectItem value="ORDER">Entire Order</SelectItem>
                    <SelectItem value="LINE_ITEM">Specific Items</SelectItem>
                    <SelectItem value="SHIPPING">Shipping Cost</SelectItem>
                </SelectContent>
                </Select>
            </div>
          </CardContent>
        </Card>

         {/* Section: Conditions & Limits */}
        <Card>
            <CardHeader>
                <CardTitle>Conditions & Limits</CardTitle>
                <CardDescription>Set rules for when and how the coupon can be used.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
                 <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2"><Label>Minimum Order Value</Label><Input type="number" placeholder="e.g., 1000" /></div>
                    <div className="space-y-2"><Label>Max Discount Value (Optional)</Label><Input type="number" placeholder="e.g., 500" /></div>
                    <div className="space-y-2"><Label>Max Total Redemptions</Label><Input type="number" placeholder="e.g., 1000" /></div>
                    <div className="space-y-2"><Label>Max Redemptions Per Member</Label><Input type="number" placeholder="e.g., 1" /></div>
                 </div>
                 <Separator />
                 <div className="space-y-2">
                    <Label>Eligibility</Label>
                    <CardDescription>Limit which members can use this coupon. Leave blank for all members.</CardDescription>
                    <div className="flex flex-wrap gap-4 pt-2">
                        <div className="flex items-center space-x-2"><Checkbox id="tier-gold" /><Label htmlFor="tier-gold">Gold Tier</Label></div>
                        <div className="flex items-center space-x-2"><Checkbox id="tier-silver" /><Label htmlFor="tier-silver">Silver Tier</Label></div>
                        <div className="flex items-center space-x-2"><Checkbox id="tag-vip" /><Label htmlFor="tag-vip">VIP Tag</Label></div>
                    </div>
                 </div>
            </CardContent>
        </Card>

        {/* Section: Validity */}
        <Card>
            <CardHeader>
                <CardTitle>Validity</CardTitle>
                <CardDescription>Define the date range when this coupon is active.</CardDescription>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="space-y-2"><Label>Valid From</Label><Input type="datetime-local" required /></div>
                <div className="space-y-2"><Label>Valid Until</Label><Input type="datetime-local" required /></div>
            </CardContent>
        </Card>

        <div className="flex justify-end gap-2">
            <Button variant="outline">Cancel</Button>
            <Button variant="secondary">Save as Draft</Button>
            <Button>Save & Activate</Button>
        </div>
      </div>
    </div>
  );
}
