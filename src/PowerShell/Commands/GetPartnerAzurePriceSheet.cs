// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.Store.PartnerCenter.PowerShell.Commands
{
    using System.Management.Automation;
    using Azure.Management.Billing;
    using Azure.Management.Billing.Models;
    using Models.Authentication;

    /// <summary>
    /// Gets the price sheet download URL for a billing profile or a specific invoice.
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "PartnerAzurePriceSheet", DefaultParameterSetName = "ByBillingProfile")]
    [OutputType(typeof(DownloadUrl))]
    public class GetPartnerAzurePriceSheet : PartnerAsyncCmdlet
    {
        /// <summary>
        /// Gets or sets the name for the billing account.
        /// </summary>
        [Parameter(HelpMessage = "The name for the billing account.", Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string BillingAccountName { get; set; }

        /// <summary>
        /// Gets or sets the name for the billing profile.
        /// </summary>
        [Parameter(HelpMessage = "The name for the billing profile.", Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string BillingProfileName { get; set; }

        /// <summary>
        /// Gets or sets the name of the invoice.
        /// </summary>
        [Parameter(HelpMessage = "The name of the invoice.", Mandatory = true, ParameterSetName = "ByInvoice")]
        [ValidateNotNullOrEmpty]
        public string InvoiceName { get; set; }

        /// <summary>
        /// Executes the operations associated with the cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            Scheduler.RunTask(async () =>
            {
                IBillingManagementClient client = await PartnerSession.Instance.ClientFactory.CreateServiceClientAsync<BillingManagementClient>(new[] { $"{PartnerSession.Instance.Context.Environment.AzureEndpoint}/user_impersonation" }).ConfigureAwait(false);

                DownloadUrl result;

                if (ParameterSetName == "ByInvoice")
                {
                    result = await client.PriceSheet.DownloadAsync(BillingAccountName, BillingProfileName, InvoiceName, CancellationToken).ConfigureAwait(false);
                }
                else
                {
                    result = await client.PriceSheet.DownloadByBillingProfileAsync(BillingAccountName, BillingProfileName, CancellationToken).ConfigureAwait(false);
                }

                WriteObject(result);
            }, true);
        }
    }
}
