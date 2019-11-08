using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using System.Threading;

namespace SBReceiveQueueConApp
{
  class Program
  {
    static NamespaceManager _namespaceManager;
    static void Main(string[] args)
    {
      CollectSBDetails();
      CreateQueueToRead();
      //Console.WriteLine("Read all the Data");
      Console.ReadKey(true);
    }

    private static void CreateQueueToRead()
    {
      TokenProvider tokenProvider = _namespaceManager.Settings.TokenProvider;
      if (_namespaceManager.QueueExists("categoryqueue"))
      {
        //Creating the queues with details
        //QueueDescription mnQueue = namespaceManagerClient.CreateQueue("categoryqueue");

        MessagingFactory factory = MessagingFactory.Create(_namespaceManager.Address, tokenProvider);

        //ReceiveAndDelete chances of not clearing the data if it crash before
        //PeekLock, it locks the second message in line and only by calling Complete it moves to the next only then it deletes it of this is default mode
        QueueClient catsQueueClient = factory.CreateQueueClient("categoryqueue");


        Console.WriteLine("Receiving the Messages from the Queue....");
        BrokeredMessage message;
        int ctr = 1;
        while ((message = catsQueueClient.Receive(new TimeSpan(hours: 0, minutes: 1, seconds: 5))) != null)
        {
          //Console.WriteLine("Message Received, Sequence: {0}, Cat: {1} and MessageID: {2}", message.SequenceNumber, message.Properties[(ctr++).ToString()], message.MessageId);
          Console.WriteLine($"Message Received, Sequence: {message.SequenceNumber}, MessageID: {message.MessageId},\nCat: {message.Properties[(ctr++).ToString()]}");
          message.Complete();
          Console.WriteLine("Processing Message (sleeping).....");
          Thread.Sleep(2000);
        }
        factory.Close();
        catsQueueClient.Close();
        _namespaceManager.DeleteQueue("categoryqueue");
        Console.WriteLine("Finished getting all the data from the queue, Press any key to exit");
      }
    }

    private static void CollectSBDetails()
    {
      _namespaceManager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"].ToString());
    }
  }
}
