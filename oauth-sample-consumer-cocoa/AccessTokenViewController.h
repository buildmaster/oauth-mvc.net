//
//  AccessTokenViewController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <OAuthConsumer/OAuthConsumer.h>
#import "ManagingViewController.h"


@interface AccessTokenViewController : ManagingViewController {
	IBOutlet NSTextField *requestTokenSecret;
	IBOutlet NSTextField *requestTokenKey;
	IBOutlet NSTextField *accessTokenKey;
	IBOutlet NSTextField *accessTokenSecret;
}
-(IBAction) getAccessToken:(id)sender;
-(void)accessTokenTicket:(OAServiceTicket *)ticket
		 didFailWithError:(NSError *) error;
-(void)accessTokenTicket:(OAServiceTicket *)ticket didFinishWithData:(NSData *)data;
-(IBAction) validateAccessToken:(id)sender;
-(IBAction) goToRequestToken:(id)sender;
@end
