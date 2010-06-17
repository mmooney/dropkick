// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace dropkick.Tasks.Security.Msmq
{
    using System.Messaging;
    using DeploymentModel;

    public class MsmqGrantReadWriteTask :
        BaseTask
    {
        public string Group;
        public string ServerName;
        public string QueueName;
        public bool PrivateQueue;

        public override string Name
        {
            get { return "Grant read/write to '{0}' for queue '{1}'".FormatWith(Group, QueuePath); }
        }

        string QueuePath {get{ return @"{0}\{1}{2}".FormatWith(ServerName, (PrivateQueue ? @"Private$\" : string.Empty), QueueName);}}

        public override DeploymentResult VerifyCanRun()
        {
            var result = new DeploymentResult();

            VerifyInAdministratorRole(result);

            return result;
        }

        public override DeploymentResult Execute()
        {
            var result = new DeploymentResult();

            var q = new MessageQueue(QueuePath);
            q.SetPermissions(Group, MessageQueueAccessRights.PeekMessage, AccessControlEntryType.Allow);
            q.SetPermissions(Group, MessageQueueAccessRights.ReceiveMessage, AccessControlEntryType.Allow);
            q.SetPermissions(Group, MessageQueueAccessRights.GetQueuePermissions, AccessControlEntryType.Allow);
            q.SetPermissions(Group, MessageQueueAccessRights.GetQueueProperties, AccessControlEntryType.Allow);
            q.SetPermissions(Group, MessageQueueAccessRights.WriteMessage, AccessControlEntryType.Allow);

            result.AddGood("Successfully granted Read/Write permissions to '{0}' for queue '{1}'".FormatWith(Group, QueuePath));

            return result;
        }
    }
}