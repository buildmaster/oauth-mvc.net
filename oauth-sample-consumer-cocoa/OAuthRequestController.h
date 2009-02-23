//
//  OAuthRequestController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 23/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import <OAuthConsumer/OAuthConsumer.h>
#import "ManagingViewController.h"

@interface OAuthRequestController : ManagingViewController {
	IBOutlet NSTextField *oAuthURL;
	IBOutlet NSTextField *output;
}

-(IBAction)makeOAuthRequest:(id)sender;
-(IBAction)back:(id)sender;
@end
