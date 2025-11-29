'use client';
import { useState } from 'react';
import { Card, CardContent, CardFooter, CardHeader, CardTitle } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { Checkbox } from '@/components/ui/checkbox';
import { IconPlus, IconTrash } from '@tabler/icons-react';

const steps = [
  'Basics',
  'Audience',
  'Rules',
  'Rewards',
  'Schedule',
  'Review & Launch',
];

export function CampaignWizard() {
  const [step, setStep] = useState(1);

  const nextStep = () => setStep((prev) => Math.min(prev + 1, steps.length));
  const prevStep = () => setStep((prev) => Math.max(prev - 1, 1));

  return (
    <div>
      <div className="mb-8 flex justify-center">
        <ol className="flex items-center w-full max-w-3xl">
          {steps.map((title, index) => (
            <li
              key={title}
              className={`flex w-full items-center ${step > index ? 'text-blue-600 dark:text-blue-500 after:border-blue-100 dark:after:border-blue-800' : 'text-gray-500 after:border-gray-200 dark:after:border-gray-700'} after:content-[''] after:w-full after:h-1 after:border-b after:border-4 after:inline-block`}
            >
              <div className="flex flex-col items-center w-24">
                <span
                  className={`flex items-center justify-center w-10 h-10 ${step >= index + 1 ? 'bg-blue-100 dark:bg-blue-800' : 'bg-gray-100 dark:bg-gray-700'} rounded-full lg:h-12 lg:w-12 shrink-0`}
                >
                  {step > index + 1 ? <svg className="w-4 h-4 text-blue-600 dark:text-blue-300" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 16 12"><path stroke="currentColor" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M1 5.917 5.724 10.5 15 1.5"/></svg> : index + 1}
                </span>
                <span className="text-xs text-center mt-2 ">{title}</span>
              </div>
            </li>
          ))}
        </ol>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>{steps[step - 1]}</CardTitle>
        </CardHeader>
        <CardContent className="min-h-[300px]">
          {step === 1 && (
            <div className="space-y-4 max-w-lg mx-auto">
              <div>
                <Label htmlFor="name">Name</Label>
                <Input id="name" minLength={5} required placeholder="e.g., Summer Weekend Bonus"/>
              </div>
              <div>
                <Label htmlFor="description">Description (Optional)</Label>
                <Textarea id="description" placeholder="A brief description of the campaign."/>
              </div>
              <div>
                <Label htmlFor="type">Type</Label>
                <Select required>
                  <SelectTrigger id="type">
                    <SelectValue placeholder="Select a campaign type" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="points-multiplier">Points Multiplier</SelectItem>
                    <SelectItem value="bonus-points">Bonus Points</SelectItem>
                    <SelectItem value="cashback">Cashback</SelectItem>
                    <SelectItem value="coupon">Coupon</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
          )}
          {step === 2 && (
            <div className="space-y-4 max-w-lg mx-auto">
                <div className="grid grid-cols-2 gap-4">
                    <div className="space-y-2">
                        <Label>Tiers</Label>
                        <div className="flex items-center space-x-2"><Checkbox id="tier-gold" /><Label htmlFor="tier-gold">Gold</Label></div>
                        <div className="flex items-center space-x-2"><Checkbox id="tier-silver" /><Label htmlFor="tier-silver">Silver</Label></div>
                        <div className="flex items-center space-x-2"><Checkbox id="tier-bronze" /><Label htmlFor="tier-bronze">Bronze</Label></div>
                    </div>
                    <div className="space-y-2">
                        <Label>Tags</Label>
                        <div className="flex items-center space-x-2"><Checkbox id="tag-vip" /><Label htmlFor="tag-vip">VIP</Label></div>
                        <div className="flex items-center space-x-2"><Checkbox id="tag-new" /><Label htmlFor="tag-new">New</Label></div>
                    </div>
                </div>
                <div><Label htmlFor="join-date">Join Date (is after)</Label><Input id="join-date" type="date" /></div>
                <div><Label>Activity</Label><Select><SelectTrigger><SelectValue placeholder="Select activity" /></SelectTrigger><SelectContent><SelectItem value="active">Active</SelectItem><SelectItem value="inactive">Inactive</SelectItem></SelectContent></Select></div>
                <div><Label>Region/Store</Label><Select><SelectTrigger><SelectValue placeholder="Select region or store" /></SelectTrigger><SelectContent><SelectItem value="north">North</SelectItem><SelectItem value="south">South</SelectItem></SelectContent></Select></div>
                 <div className="text-sm font-semibold text-gray-500 dark:text-gray-400 pt-4">Estimated reach: 1234 members</div>
            </div>
          )}
          {step === 3 && (
             <div className="space-y-4 max-w-2xl mx-auto">
                <div className="p-4 border rounded-md bg-slate-50 dark:bg-slate-800/50">
                    <div className="flex items-center space-x-2 mb-4">
                        <Label>IF</Label>
                        <Select defaultValue="and">
                            <SelectTrigger className="w-auto"><SelectValue /></SelectTrigger>
                            <SelectContent><SelectItem value="and">All (AND)</SelectItem><SelectItem value="or">Any (OR)</SelectItem></SelectContent>
                        </Select>
                        <Label>of the following conditions are met:</Label>
                    </div>

                    <div className="space-y-4 pl-8">
                        {/* Rule 1 */}
                        <div className="flex items-center space-x-2">
                           <Select defaultValue="purchase-completed"><SelectTrigger className="w-1/3"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="purchase-completed">Purchase Completed</SelectItem><SelectItem value="birthday-month">Birthday Month</SelectItem><SelectItem value="tier-change">Tier Change</SelectItem><SelectItem value="tag-added">Tag Added</SelectItem><SelectItem value="login">Login</SelectItem></SelectContent></Select>
                           <Select defaultValue="greater-than-equal"><SelectTrigger className="w-auto"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="equals">=</SelectItem><SelectItem value="not-equals">!=</SelectItem><SelectItem value="greater-than">&gt;</SelectItem><SelectItem value="greater-than-equal">&gt;=</SelectItem><SelectItem value="less-than">&lt;</SelectItem><SelectItem value="less-than-equal">&lt;=</SelectItem><SelectItem value="contains">contains</SelectItem><SelectItem value="in-list">in list</SelectItem></SelectContent></Select>
                           <Input placeholder="Value" className="w-1/3"/>
                           <Button variant="ghost" size="icon"><IconTrash className="h-4 w-4 text-red-500" /></Button>
                        </div>
                        {/* Rule 2 */}
                        <div className="flex items-center space-x-2">
                           <Select defaultValue="purchase-completed"><SelectTrigger className="w-1/3"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="purchase-completed">Purchase Completed</SelectItem></SelectContent></Select>
                           <Select defaultValue="contains"><SelectTrigger className="w-auto"><SelectValue /></SelectTrigger><SelectContent><SelectItem value="contains">contains category</SelectItem></SelectContent></Select>
                           <Input placeholder="e.g., Electronics" className="w-1/3"/>
                           <Button variant="ghost" size="icon"><IconTrash className="h-4 w-4 text-red-500" /></Button>
                        </div>

                         <div className="pt-2"><Button variant="outline" size="sm"><IconPlus className="h-4 w-4 mr-2" />Add Condition</Button></div>
                    </div>
                </div>
            </div>
          )}
          {step === 4 && (
             <div className="space-y-4 max-w-lg mx-auto">
              <p className="text-sm text-center text-gray-500 dark:text-gray-400">Define the reward that members will receive when they meet the rules defined in the previous step.</p>
              <div><Label>Points Multiplier</Label><Input type="number" min="1.1" max="5.0" step="0.1" placeholder="e.g., 2.5" /></div>
              <div><Label>Bonus Points</Label><Input type="number" placeholder="e.g., 500" /></div>
              <div><Label>Cashback</Label><div className="flex items-center space-x-2"><Input type="number" placeholder="Percentage" /><span className="text-gray-500">or</span><Input type="number" placeholder="Flat amount" /></div></div>
              <div><Label>Coupon</Label><Select><SelectTrigger><SelectValue placeholder="Select a coupon from rewards catalog" /></SelectTrigger><SelectContent><SelectItem value="summer-promo">Summer Promo</SelectItem><SelectItem value="weekend-deal">Weekend Deal</SelectItem></SelectContent></Select></div>
              <div><Label>Destination wallet</Label><Select><SelectTrigger><SelectValue placeholder="Select a wallet" /></SelectTrigger><SelectContent><SelectItem value="points">Points</SelectItem><SelectItem value="cashback">Cashback</SelectItem><SelectItem value="coupon">Coupon</SelectItem></SelectContent></Select></div>
            </div>
          )}
          {step === 5 && (
            <div className="space-y-4 max-w-lg mx-auto">
              <div className="grid grid-cols-2 gap-4">
                <div><Label htmlFor="start-date">Start Date & Time</Label><Input id="start-date" type="datetime-local" required /></div>
                <div><Label htmlFor="end-date">End Date & Time</Label><Input id="end-date" type="datetime-local" required /></div>
              </div>
              <div><Label>Recurrence</Label><Select><SelectTrigger><SelectValue placeholder="No recurrence" /></SelectTrigger><SelectContent><SelectItem value="none">None</SelectItem><SelectItem value="weekends">Weekends</SelectItem><SelectItem value="weekdays">Weekdays</SelectItem></SelectContent></Select></div>
            </div>
          )}
          {step === 6 && (
            <div className="space-y-4 max-w-lg mx-auto">
              <Card className="bg-slate-50 dark:bg-slate-800/50">
                <CardHeader><CardTitle>Summer 2X</CardTitle></CardHeader>
                <CardContent className="space-y-2 text-sm">
                  <p><strong>Description:</strong> 2x points on all purchases for Gold Tier members during the first 10 days of June.</p>
                  <p><strong>Type:</strong> Multiplier</p>
                  <p><strong>Duration:</strong> 01 Jun 2025, 12:00 AM to 10 Jun 2025, 11:59 PM</p>
                  <p><strong>Audience:</strong> Gold Tier Members</p>
                  <p><strong>Rules:</strong> Purchase Completed</p>
                  <p><strong>Reward:</strong> 2x Points to Points Wallet</p>
                </CardContent>
              </Card>
            </div>
          )}
        </CardContent>
        <CardFooter className="flex justify-between">
          <Button variant="outline" onClick={prevStep} disabled={step === 1}>Previous</Button>
          <div>
            {step === steps.length && <Button variant="secondary" className="mr-2">Save Draft</Button>}
            {step === steps.length ? <Button>Activate</Button> : <Button onClick={nextStep}>Next</Button>}
          </div>
        </CardFooter>
      </Card>
    </div>
  );
}
