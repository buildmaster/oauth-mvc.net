//
//  RequestTokenController.m
//  oauth-demo-consumer
//
//  Created by Owen Evans on 19/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "RequestTokenViewController.h"

@implementation RequestTokenViewController

-(id) init
{
	if(![super initWithNibName:@"RequestTokenView" bundle:nil])
	{
		return nil;
	}
	[self setTitle:@"Get Request Token"];
	[nextView setEnabled:FALSE];
	return self;
}



- (IBAction) getRequestKey:(id) sender
{
	OAConsumer *consumer;
	NSString *consumerKeyValue = [consumerKey stringValue];
	NSString *consumerSecretValue = [consumerSecret stringValue];
	[parent setSharedValue:consumerKeyValue forKey:OACConsumerKey];
	[parent setSharedValue:consumerSecretValue forKey:OACConsumerSecret];
	consumer = [[OAConsumer alloc] initWithKey:consumerKeyValue secret:consumerSecretValue];
	NSURL *requestUrl = [NSURL URLWithString:@"http://172.19.105.240/oauth/RequestToken"];
	OAMutableURLRequest *request = [[OAMutableURLRequest alloc] initWithURL:requestUrl
																   consumer:consumer
																	  token:nil
																	  realm:nil
														  signatureProvider:nil];
	NSLog(@"Using URL: %@",[request URL]);
	[request setHTTPMethod:@"POST"];
	OADataFetcher *fetcher = [[OADataFetcher alloc] init];
    [fetcher fetchDataWithRequest:request
                         delegate:self
                didFinishSelector:@selector(requestTokenTicket:didFinishWithData:)
                  didFailSelector:@selector(requestTokenTicket:didFailWithError:)];
	[consumer release];
	[OAMutableURLRequest release];
	[OADataFetcher release];
		
	}
-(void)requestTokenTicket:(OAServiceTicket *)ticket 
		didFinishWithData:(NSData *)data
{
	NSLog(@"going through success message");
	if (ticket.didSucceed) {
		NSString *responseBody = [[NSString alloc] initWithData:data
													   encoding:NSUTF8StringEncoding];
		OAToken *requestToken = [[OAToken alloc] initWithHTTPResponseBody:responseBody];
		NSString *secret = [requestToken secret];
		NSString *key = [requestToken key];
		NSLog(@"Token recieved with token %@ and secret %@",key,secret);
		[parent setSharedValue:key forKey:OACRequestTokenKey];
		[parent setSharedValue:secret forKey:OACRequestTokenSecret];
		[token setStringValue:key];
		[tokenSecret setStringValue:secret];
		[nextView setEnabled:TRUE];
		[requestToken release];
	}
	else {
		NSLog(@"ticket failed"); 
	}
	
}
-(void)requestTokenTicket:(OAServiceTicket *)ticket
didFailWithError:(NSError *) error
{
	NSLog(@"going through fail message");
	//NSLog([error localizedDescription]);
	NSLog([error localizedFailureReason]);

}
-(IBAction) moveToGetAccessKey:(id) sender
{
	[parent setView:@"GetAccessToken"];
	
}
	
@end

