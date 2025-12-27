"use client"

import Link from "next/link"
import { usePathname } from "next/navigation"
import { Gift, Home, User, Wallet, Menu, Star } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { useAuth } from "@/hooks/use-auth"
import { ModeToggle } from "./mode-toggle"
import { ThemeSelector } from "./theme-selector"

export function DashboardNav() {
    const pathname = usePathname()
    const { member, logout } = useAuth()

    const links = [
        { href: "/dashboard", label: "Dashboard", icon: Home },
        { href: "/earn", label: "Earn Points", icon: Star },
        { href: "/rewards", label: "Rewards", icon: Gift },
        { href: "/history", label: "History", icon: Wallet },
        { href: "/profile", label: "Profile", icon: User },
    ]

    return (
        <header className="sticky top-0 z-30 flex h-[var(--header-height)] items-center gap-4 border-b bg-background/95 backdrop-blur-sm px-6">
            <Sheet>
                <SheetTrigger asChild>
                    <Button variant="outline" size="icon" className="md:hidden">
                        <Menu className="h-5 w-5" />
                        <span className="sr-only">Toggle navigation</span>
                    </Button>
                </SheetTrigger>
                <SheetContent side="left">
                    <nav className="grid gap-6 text-lg font-medium">
                        <Link href="#" className="flex items-center gap-2 text-lg font-semibold">
                            <span className="text-primary">Open Loyalty</span>
                        </Link>
                        {links.map((link) => (
                            <Link
                                key={link.href}
                                href={link.href}
                                className={`flex items-center gap-4 hover:text-foreground ${pathname === link.href ? "text-foreground" : "text-muted-foreground"
                                    }`}
                            >
                                <link.icon className="h-5 w-5" />
                                {link.label}
                            </Link>
                        ))}
                    </nav>
                </SheetContent>
            </Sheet>

            <div className="flex w-full items-center gap-4 md:gap-8 md:ml-auto">
                <div className="ml-auto flex items-center gap-4">
                    {/* Desktop Nav (Hidden on Mobile) */}
                    <div className="flex items-center gap-2">
                        <ModeToggle />
                        <ThemeSelector />
                    </div>

                    <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                            <Button variant="secondary" size="icon" className="rounded-full">
                                <Avatar>
                                    {/* Placeholder generic avatar */}
                                    <AvatarImage src="" />
                                    <AvatarFallback>{member?.firstName?.[0] || "M"}</AvatarFallback>
                                </Avatar>
                                <span className="sr-only">Toggle user menu</span>
                            </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                            <DropdownMenuLabel>My Account</DropdownMenuLabel>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem onClick={() => logout()}>Logout</DropdownMenuItem>
                        </DropdownMenuContent>
                    </DropdownMenu>
                </div>
            </div>
        </header>
    )
}
