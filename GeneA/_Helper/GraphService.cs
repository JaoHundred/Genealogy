using AvaloniaGraphControl;
using Model.Interfaces;
using ModelA.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneA._Helper
{
    public class GraphService
    {
        public GraphService(IRepository<Person> repository)
        {
            _repository = repository;

            _graph = new Graph();
        }

        private readonly IRepository<Person> _repository;

        private Graph _graph;

        private bool _adjustCount = false;

        public async Task<Graph> ToGraphAsync(Person person, int generations, int generationCount = 0)
        {
            //TODO: graph is not rendering updated entity
            //steps to reproduce: go to any person, open the family tree, click on any node and edit some visual property(name or dates)
            //click in save and you will be sent back to the tree, the edited note should have been visualiy updated but its not


            if(!_adjustCount)
            {
                //the graph can only render entities if it has at least 2 generations, this portion of code
                //make the visual rendered entities match the selected number of generations

                generationCount++;
                _adjustCount = true;
            }

            return await Task.Run(async () =>
              {
                  generationCount++;

                  if (generationCount > generations)
                      return _graph;

                  if (person.Father != null && person.Mother != null)
                  {
                      person.Father = _repository.FindById(person.Father.Id);
                      person.Mother = _repository.FindById(person.Mother.Id);

                      var fatherEdge = new Edge(person.Father, person);
                      var motherEdge = new Edge(person.Mother, person);

                      var spouse = new Edge(person.Father, person.Mother, headSymbol: Edge.Symbol.Arrow, tailSymbol: Edge.Symbol.Arrow
                          ,label: DynamicTranslate.Translate(MessageConsts.Spouse));

                      _graph.Edges.Add(spouse);

                      _graph.Edges.Add(fatherEdge);
                      _graph.Edges.Add(motherEdge);

                      
                      await ToGraphAsync(person.Father, generations, generationCount);
                      await ToGraphAsync(person.Mother, generations, generationCount);
                  }

                  //TODO: sometimes graph changes its order, see why this is happening

                  return _graph;
              });
        }
    }
}
