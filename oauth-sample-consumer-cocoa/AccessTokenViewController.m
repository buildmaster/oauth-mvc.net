//
//  AccessTokenViewController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "AccessTokenViewController.h"
#import "ApplicationController.h"

@implementation AccessTokenViewController
-(id) init
{
	if(![super initWithNibName:@"AccessTokenView" bundle:nil])
	{
		return nil;
	}
	[self setTitle:@"Get Access Token"];
	
	return self;
}
-(void) loadView
{
	[super loadView];
	NSString *consumerKeyValue = [parent getSharedValue:OACConsumerKey];
	NSLog(@"Loaded Key from store %@",consumerKeyValue);
	[requestTokenKey setStringValue:[parent getSharedValue:OACRequestTokenKey]];
	[requestTokenSecret setStringValue:[parent getSharedValue:OACRequestTokenSecret]];
	[requestTokenKey setEditable:FALSE];
	[requestTokenSecret setSelectable:FALSE];
	[accessTokenKey setEditable:FALSE];
	[accessTokenSecret setSelectable:FALSE];
}
-(IBAction) getAccessToken:(id)sender
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	
	NSString *consumerKeyValue = [defaults valueForKey:OACConsumerKey];
	NSString *consumerSecretValue = [defaults valueForKey:OACConsumerSecret];
	NSLog(@"Loaded consumer key from screen %@",consumerKeyValue);
	NSLog(@"Loaded consumer secret from screen %@",consumerSecretValue);
	OAConsumer *consumer = [[OAConsumer alloc] initWithKey:consumerKeyValue secret:consumerSecretValue];
	OAToken *requestToken = [[OAToken alloc] initWithKey:[requestTokenKey stringValue] secret:[requestTokenSecret stringValue]];
	NSLog(@"created token");
	NSURL *requestUrl = [NSURL URLWithString:[defaults valueForKey:OACAccessTokenUrl]];
	NSLog(@"Created URL");
	OAMutableURLRequest *request = [[OAMutableURLRequest alloc]initWithURL:requestUrl
																  consumer:consumer
																	 token:requestToken
																	 realm:nil
														 signatureProvider:nil];
	NSLog(@"Created Mutable URL Request");
	[request setHTTPMethod:@"GET"];
	NSLog(@"Created Request");
	OADataFetcher *fetcher = [[OADataFetcher alloc]init];
	NSLog(@"Created fetcher"); 
    [fetcher fetchDataWithRequest:request
                         delegate:self
                didFinishSelector:@selector(accessTokenTicket:didFinishWithData:)
                  didFailSelector:@selector(accessTokenTicket:didFailWithError:)];
	NSLog(@"initialised fetcher");
	[fetcher release];
	[request release];
	[consumer release];
}
-(void) accessTokenTicket: (OAServiceTicket *) ticket
didFinishWithData:(NSData *)data
{
	NSLog(@"Got Access ticket");
	if(ticket.didSucceed)
	{
		NSLog(@"Ticket succeeded");
		NSString *responseBody = [[NSString alloc] initWithData:data
													   encoding:NSUTF8StringEncoding];
		OAToken *accessToken = [[OAToken alloc] initWithHTTPResponseBody:responseBody];
		NSLog(@"Got Ticket Key %@ and secret %@",[accessToken key],[accessToken secret]);
		[accessTokenKey setStringValue:[accessToken key]];
		[accessTokenSecret setStringValue:[accessToken secret]];
	}
	else
	{
		NSLog(@"Ticket didn't succeed");
	}
}
-(void) accessTokenTicket: (OAServiceTicket *) ticket
		didFailWithError:(NSError *)error
{
	NSLog(@"Access token request returned error");
	NSLog(@"Error = %@",[error localizedDescription]);

}
-(IBAction) validateAccessToken:(id)sender
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	
	NSString *urlString = [NSString stringWithFormat:[defaults valueForKey:	OACRequestTokenAuthUrl],[requestTokenKey stringValue]];
	NSURL *url = [NSURL URLWithString:urlString];
	[[NSWorkspace sharedWorkspace] openURL:url];
	
}
-(IBAction) goToRequestToken:(id)sender
{
	
	[parent setViewName:@"GetRequestToken"];
}
@end
