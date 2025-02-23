
import type { Metadata } from "next";
import NavigationBar from "./Nav/NavigationBar";
import "./globals.css";

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  console.log('trigger server')
  return (
    <html lang="en">
      <body>
        <NavigationBar></NavigationBar>
        <main className="container mx-auto px-5">
        {children}
        </main>
        
      </body>
    </html>
  );
}
