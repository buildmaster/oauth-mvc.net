//
//  OAuthRequestController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 23/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "OAuthRequestController.h"
#import "ApplicationController.h"


@implementation OAuthRequestController
-(id) init
{
	if(![super initWithNibName:@"OAuthRequest" bundle:nil])
	{
		return nil;
	}
	[self setTitle:@"Make OAuth Request"];
	
	return self;
}
-(void) loadView
{
	[super loadView];	
	NSUserDefaults *defaults =[NSUserDefaults standardUserDefaults];
	[oAuthURL setStringValue:[defaults valueForKey:OACLastRequestUrl]];
}
-(IBAction)makeOAuthRequest:(id)sender
{
	NSUserDefaults *defaults = [NSUserDefaults standardUserDefaults];
	NSString *consumerKey = [defaults valueForKey:OACConsumerKey];
	NSString *consumerSecret = [defaults valueForKey:OACConsumerSecret];
	[defaults setObject:[oAuthURL stringValue] forKey:OACLastRequestUrl];
	NSString *accessTokenKey = [parent getSharedValue:OACAccessTokenKey];
	NSString *accessTokenSecret = [parent getSharedValue:OACAccessTokenSecret];
	OAConsumer *consumer = [[OAConsumer alloc] initWithKey:consumerKey secret:consumerSecret];
	OAToken *accessToken = [[OAToken alloc] initWithKey:accessTokenKey secret:accessTokenSecret];
	NSURL *queryUrl = [NSURL URLWithString:[oAuthURL stringValue]];
	OAMutableURLRequest *requestUrl = [[OAMutableURLRequest alloc] initWithURL:queryUrl	
																	  consumer:consumer 
																		 token:accessToken
																		 realm:nil
															 signatureProvider:nil];
	
	NSLog(@"Created Mutable request to %@",[requestUrl URL]);
	[requestUrl setHTTPMethod:@"GET"];
	OADataFetcher *fetcher = [[OADataFetcher alloc]init];
	NSLog(@"Created fetcher");
	[fetcher fetchDataWithRequest:requestUrl
                         delegate:self
                didFinishSelector:@selector(accessTokenTicket:didFinishWithData:)
                  didFailSelector:@selector(accessTokenTicket:didFailWithError:)];
	[fetcher release];
	[requestUrl release];
	[consumer release];
	
}
-(void) accessTokenTicket: (OAServiceTicket *) ticket
		didFinishWithData: (NSData *)data
{
	NSLog(@"Made Request");
	if(ticket.didSucceed)
	{
		NSLog(@"Ticket succeeded");
		NSString *responseBody = [[NSString alloc] initWithData:data
													   encoding:NSUTF8StringEncoding];
		NSLog(@"got response %@",responseBody);
		[output setStringValue:responseBody];
	}
	else
	{
		NSLog(@"Ticket didn't succeed");
		NSString *responseBody = [[NSString alloc] initWithData:data
													   encoding:NSUTF8StringEncoding];
		NSLog(@"got response %@",responseBody);
	}
}
-(void) accessTokenTicket: (OAServiceTicket *) ticket
		 didFailWithError:(NSError *)error
{
	NSLog(@"Error in request returned error");
	NSLog(@"Error = %@",[error localizedDescription]);
}
-(IBAction)back:(id)sender
{
	[parent setViewName:@"GetAccessToken"];
}
@end
