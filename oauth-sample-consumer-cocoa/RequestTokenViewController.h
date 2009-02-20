//
//  RequestTokenController.h
//  oauth-demo-consumer
//
//  Created by Owen Evans on 19/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <OAuthConsumer/OAuthConsumer.h>
#import "ManagingViewController.h"

@interface RequestTokenViewController : ManagingViewController {
	IBOutlet NSTextField *consumerKey;
	IBOutlet NSTextField *consumerSecret;
	IBOutlet NSButton *nextView;
	IBOutlet NSTextField *token;
	IBOutlet NSTextField *tokenSecret;
}

-(IBAction) getRequestKey:(id) sender;
-(IBAction) moveToGetAccessKey:(id) sender;
-(void)requestTokenTicket:(OAServiceTicket *)ticket
		 didFailWithError:(NSError *) error;
- (void)requestTokenTicket:(OAServiceTicket *)ticket didFinishWithData:(NSData *)data;
@end
